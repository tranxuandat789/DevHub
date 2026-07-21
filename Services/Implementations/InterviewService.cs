//12/07/2026 PhongDH
using System;
using System.Threading.Tasks;
using DevHub.Data;
using DevHub.Models;
using DevHub.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DevHub.Services.Implementations;

public class InterviewService : IInterviewService
{
    private readonly ItrecruitmentDbContext _context;
    private readonly ILogger<InterviewService> _logger;
    private readonly DevHub.Helpers.EmailHelper _emailHelper;
    private readonly INotificationService _notificationService;

    public InterviewService(ItrecruitmentDbContext context, ILogger<InterviewService> logger, DevHub.Helpers.EmailHelper emailHelper, INotificationService notificationService)
    {
        _context = context;
        _logger = logger;
        _emailHelper = emailHelper;
        _notificationService = notificationService;
    }

    public async Task<Interview> CreateInterviewAsync(int recruiterId, int applicationId, DateTime scheduledTime, string interviewType, string locationOrLink, string? notes)
    {
        var application = await _context.Applications
            .Include(a => a.Job)
            .Include(a => a.Candidate).ThenInclude(c => c.CandidateNavigation)
            .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);
            
        if (application == null) throw new Exception("Không tìm thấy đơn ứng tuyển");

        var meetingLink = interviewType.ToUpper() == "ONLINE" ? locationOrLink : null;
        var location = interviewType.ToUpper() == "OFFLINE" ? locationOrLink : null;

        var interview = new Interview
        {
            ApplicationId = applicationId,
            CandidateId = application.CandidateId,
            RecruiterId = recruiterId,
            ScheduledTime = scheduledTime,
            InterviewType = interviewType,
            MeetingLink = meetingLink,
            Location = location,
            Notes = notes,
            Status = "scheduled",
            CreatedAt = DateTime.Now
        };

        _context.Interviews.Add(interview);
        await _context.SaveChangesAsync();

        // Create notification for candidate
        try
        {
                
            var title = application?.Job?.Title ?? "công việc";
            var isOnline = interviewType.ToUpper() == "ONLINE";

            // In-app message: (Đã loại bỏ link họp vì đã hiển thị ở phần chi tiết bên dưới)
            var inAppMessage = $"Bạn có lịch phỏng vấn vào {scheduledTime:dd/MM/yyyy HH:mm} cho vị trí {title}.";

            await _notificationService.SendNotificationAsync(
                userId: application.CandidateId,
                userType: "CANDIDATE",
                title: $"Bạn có lịch phỏng vấn cho {title}",
                message: inAppMessage,
                type: "INTERVIEW",
                severity: "info",
                referenceId: interview.InterviewId,
                referenceType: "Interview"
            );

            var emailEnabled = application?.Candidate?.CandidateNavigation?.EmailNotificationsEnabled ?? true;
            var candidateEmail = application?.Candidate?.CandidateNavigation?.Email;
            if (emailEnabled && !string.IsNullOrWhiteSpace(candidateEmail))
            {
                var candidateName = application?.Candidate?.FullName ?? "Bạn";
                var subject = "Thông báo Lịch phỏng vấn mới - DevHub";

                // Dòng địa điểm/link tuỳ theo hình thức
                var locationRow = isOnline && !string.IsNullOrWhiteSpace(meetingLink)
                    ? $"<p>Link họp trực tuyến: <a href='{meetingLink}' style='color:#4640DE;'>{meetingLink}</a></p>"
                    : (!string.IsNullOrWhiteSpace(location)
                        ? $"<p>Địa điểm: <b>{location}</b></p>"
                        : "");

                var body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #D6DDEB; border-radius: 8px;'>
                    <h2 style='color: #4640DE;'>Bạn có Lịch phỏng vấn mới</h2>
                    <p>Chào <b>{candidateName}</b>,</p>
                    <p>Nhà tuyển dụng vừa tạo lịch phỏng vấn với bạn cho vị trí <b>{title}</b>.</p>
                    <p>Thời gian: <b>{scheduledTime:dd/MM/yyyy HH:mm}</b></p>
                    <p>Hình thức: <b>{(isOnline ? "Trực tuyến" : "Trực tiếp")}</b></p>
                    {locationRow}
                    <hr style='border: none; border-top: 1px solid #E5E5E5; margin: 20px 0;' />
                    <p style='font-size: 12px; color: #888888; text-align: center;'>Hệ thống tuyển dụng DevHub</p>
                </div>";
                
                await _emailHelper.SendEmailAsync(candidateEmail, subject, body);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to create interview notification for interview {InterviewId}", interview.InterviewId);
        }

        return interview;
    }

    public async Task<Interview> UpdateInterviewAsync(int recruiterId, int interviewId, DateTime scheduledTime, string interviewType, string locationOrLink, string? notes, string? reasonForChange = null)
    {
        var interview = await _context.Interviews
            .Include(i => i.Application).ThenInclude(a => a.Job)
            .Include(i => i.Candidate).ThenInclude(c => c.CandidateNavigation)
            .FirstOrDefaultAsync(i => i.InterviewId == interviewId && i.Application.Job.CompanyId == _context.Recruiters.Where(r => r.RecruiterId == recruiterId).Select(r => r.CompanyId).FirstOrDefault());
        if (interview == null)
            throw new Exception("Interview not found");

        bool timeChanged = interview.ScheduledTime != scheduledTime;

        var meetingLink = interviewType.ToUpper() == "ONLINE" ? locationOrLink : null;
        var location = interviewType.ToUpper() == "OFFLINE" ? locationOrLink : null;

        interview.ScheduledTime = scheduledTime;
        interview.InterviewType = interviewType;
        interview.MeetingLink = meetingLink;
        interview.Location = location;
        interview.Notes = notes;
        interview.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        // Luôn gửi thông báo/email khi có bất kỳ thay đổi nào
        try
        {
            var isOnline = interviewType.ToUpper() == "ONLINE";
            var title = interview.Application?.Job?.Title ?? "công việc";
            var reasonText = string.IsNullOrWhiteSpace(reasonForChange) ? "Không có lý do cụ thể." : reasonForChange;
            
            var notifMessage = $"Nhà tuyển dụng đã cập nhật lịch phỏng vấn cho vị trí bạn ứng tuyển. Vui lòng kiểm tra kỹ thông tin bên dưới và đảm bảo có mặt đúng thời gian theo lịch mới. Nếu có bất kỳ thắc mắc nào, vui lòng liên hệ trực tiếp với nhà tuyển dụng để được hỗ trợ. Lý do thay đổi: {reasonText}";

            await _notificationService.SendNotificationAsync(
                userId: interview.CandidateId,
                userType: "CANDIDATE",
                title: $"Thông báo về việc thay đổi lịch phỏng vấn cho công việc {title}",
                message: notifMessage,
                type: "INTERVIEW",
                severity: "warning",
                referenceId: interview.InterviewId,
                referenceType: "Interview"
            );

            var emailEnabled = interview.Candidate?.CandidateNavigation?.EmailNotificationsEnabled ?? false;
            var candidateEmail = interview.Candidate?.CandidateNavigation?.Email;
            if (emailEnabled && !string.IsNullOrWhiteSpace(candidateEmail))
            {
                var candidateName = interview.Candidate?.FullName ?? "Bạn";
                var subject = $"Thông báo về việc thay đổi lịch phỏng vấn cho công việc {title}";

                var locationRow = isOnline
                    ? $"<p>Link họp trực tuyến: <a href='{locationOrLink}' style='color:#4640DE;'>{locationOrLink}</a></p>"
                    : $"<p>Địa điểm: <b>{locationOrLink}</b></p>";

                var body = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #D6DDEB; border-radius: 8px;'>
                        <h2 style='color: #4640DE;'>Lịch phỏng vấn đã được cập nhật</h2>
                        <p>Chào <b>{candidateName}</b>,</p>
                        <p>Nhà tuyển dụng đã cập nhật lịch phỏng vấn cho vị trí bạn ứng tuyển. Vui lòng kiểm tra kỹ thông tin bên dưới và đảm bảo có mặt đúng thời gian theo lịch mới. Nếu có bất kỳ thắc mắc nào, vui lòng liên hệ trực tiếp với nhà tuyển dụng để được hỗ trợ.</p>
                        <p><b>Lý do thay đổi:</b> {reasonText}</p>
                        <p>Thời gian mới: <b>{scheduledTime:dd/MM/yyyy HH:mm}</b></p>
                        <p>Hình thức: <b>{(isOnline ? "Trực tuyến" : "Trực tiếp")}</b></p>
                        {locationRow}
                        <hr style='border: none; border-top: 1px solid #E5E5E5; margin: 20px 0;' />
                        <p style='font-size: 12px; color: #888888; text-align: center;'>Hệ thống tuyển dụng DevHub</p>
                    </div>";

                await _emailHelper.SendEmailAsync(candidateEmail, subject, body);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to create interview update notification for interview {InterviewId}", interview.InterviewId);
        }

        return interview;
    }

    public async Task<bool> UpdateStatusAsync(int recruiterId, int interviewId, string status, string? reason = null)
    {
        var interview = await _context.Interviews
            .Include(i => i.Application).ThenInclude(a => a.Job)
            .Include(i => i.Candidate).ThenInclude(c => c.CandidateNavigation)
            .FirstOrDefaultAsync(i => i.InterviewId == interviewId && i.Application.Job.CompanyId == _context.Recruiters.Where(r => r.RecruiterId == recruiterId).Select(r => r.CompanyId).FirstOrDefault());
            
        if (interview == null)
            return false;
            
        // Tránh trùng lặp nếu trạng thái đã được cập nhật trước đó
        if (interview.Status == status)
            return true;

        interview.Status = status;
        if (status == "passed" && interview.Application != null)
        {
            interview.Application.Status = "HIRED";
        }
        else if (status == "rejected" && interview.Application != null)
        {
            interview.Application.Status = "FAILED";
        }

        interview.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        if (status == "cancelled")
        {
            try
            {
                var reasonText = string.IsNullOrWhiteSpace(reason) ? "Không có lý do cụ thể." : reason;
                await _notificationService.SendNotificationAsync(
                    userId: interview.CandidateId,
                    userType: "CANDIDATE",
                    title: "Lịch phỏng vấn đã bị hủy",
                    message: $"Nhà tuyển dụng đã hủy lịch phỏng vấn của bạn đối với vị trí {interview.Application?.Job?.Title}. Vui lòng tham khảo lý do hủy lịch dưới đây. Nếu nhà tuyển dụng sắp xếp lịch phỏng vấn mới, hệ thống sẽ gửi thông báo đến bạn trong thời gian sớm nhất.\n\nLý do hủy: {reasonText}",
                    type: "INTERVIEW",
                    severity: "error",
                    referenceId: interview.InterviewId,
                    referenceType: "Interview"
                );

                var emailEnabled = interview.Candidate?.CandidateNavigation?.EmailNotificationsEnabled ?? true;
                var candidateEmail = interview.Candidate?.CandidateNavigation?.Email;
                if (emailEnabled && !string.IsNullOrWhiteSpace(candidateEmail))
                {
                    var title = interview.Application?.Job?.Title ?? "công việc";
                    var candidateName = interview.Candidate?.FullName ?? "Bạn";
                    var subject = $"Thông báo hủy lịch phỏng vấn cho công việc {title}";
                    var body = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #D6DDEB; border-radius: 8px;'>
                        <h2 style='color: #EF4444;'>Lịch phỏng vấn bị hủy</h2>
                        <p>Chào <b>{candidateName}</b>,</p>
                        <p>Nhà tuyển dụng đã hủy lịch phỏng vấn của bạn đối với vị trí <b>{title}</b>. Vui lòng tham khảo lý do hủy lịch dưới đây. Nếu nhà tuyển dụng sắp xếp lịch phỏng vấn mới, hệ thống sẽ gửi thông báo đến bạn trong thời gian sớm nhất.</p>
                        <p><b>Lý do hủy:</b> {reasonText}</p>
                        <hr style='border: none; border-top: 1px solid #E5E5E5; margin: 20px 0;' />
                        <p style='font-size: 12px; color: #888888; text-align: center;'>Hệ thống tuyển dụng DevHub</p>
                    </div>";
                    
                    await _emailHelper.SendEmailAsync(candidateEmail, subject, body);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send cancellation notification for interview {InterviewId}", interview.InterviewId);
            }
        }
        else if (status == "passed" || status == "rejected")
        {
            try
            {
                var reasonText = string.IsNullOrWhiteSpace(reason) ? "Không có lý do cụ thể." : reason;
                var title = interview.Application?.Job?.Title ?? "công việc";
                
                string notifTitle = status == "passed" ? $"Chúc mừng! Bạn đã trúng tuyển vị trí {title}" : $"Kết quả phỏng vấn vị trí {title}";
                string notifMessage = status == "passed" 
                    ? $"Nhà tuyển dụng đã xác nhận bạn trúng tuyển vị trí {title}. Chúc mừng bạn! Vui lòng chờ các thông tin tiếp theo hoặc liên hệ trực tiếp với nhà tuyển dụng để biết thêm chi tiết." + (!string.IsNullOrWhiteSpace(reason) ? $"\nGhi chú: {reason}" : "")
                    : $"Nhà tuyển dụng đã cập nhật kết quả phỏng vấn của bạn đối với vị trí {title}. Rất tiếc, bạn chưa phù hợp với vị trí này ở thời điểm hiện tại.\n\nLý do: {reasonText}\n\nChúc bạn may mắn trong những cơ hội tiếp theo.";
                string severityLevel = status == "passed" ? "success" : "error";

                await _notificationService.SendNotificationAsync(
                    userId: interview.CandidateId,
                    userType: "CANDIDATE",
                    title: notifTitle,
                    message: notifMessage,
                    type: "INTERVIEW",
                    severity: severityLevel,
                    referenceId: interview.InterviewId,
                    referenceType: "Interview"
                );

                var emailEnabled = interview.Candidate?.CandidateNavigation?.EmailNotificationsEnabled ?? true;
                var candidateEmail = interview.Candidate?.CandidateNavigation?.Email;
                if (emailEnabled && !string.IsNullOrWhiteSpace(candidateEmail))
                {
                    var candidateName = interview.Candidate?.FullName ?? "Bạn";
                    var subject = status == "passed" ? $"Chúc mừng! Bạn đã trúng tuyển công việc {title}" : $"Thông báo kết quả phỏng vấn cho công việc {title}";
                    
                    var headerStyle = status == "passed" ? "color: #10B981;" : "color: #EF4444;";
                    var headerText = status == "passed" ? "Chúc mừng bạn đã trúng tuyển!" : "Thông báo kết quả phỏng vấn";
                    var bodyContent = status == "passed" 
                        ? $"<p>Nhà tuyển dụng đã xác nhận bạn trúng tuyển vị trí <b>{title}</b>. Chúc mừng bạn!</p><p>Vui lòng chờ các thông tin tiếp theo hoặc liên hệ trực tiếp với nhà tuyển dụng để biết thêm chi tiết.</p>" + (!string.IsNullOrWhiteSpace(reason) ? $"<p><b>Ghi chú:</b> {reason}</p>" : "")
                        : $"<p>Nhà tuyển dụng đã cập nhật kết quả phỏng vấn của bạn đối với vị trí <b>{title}</b>. Rất tiếc, bạn chưa phù hợp với vị trí này ở thời điểm hiện tại.</p><p><b>Lý do:</b> {reasonText}</p><p>Cảm ơn bạn đã quan tâm và dành thời gian ứng tuyển. Chúc bạn nhiều thành công trong tương lai.</p>";

                    var body = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #D6DDEB; border-radius: 8px;'>
                        <h2 style='{headerStyle}'>{headerText}</h2>
                        <p>Chào <b>{candidateName}</b>,</p>
                        {bodyContent}
                        <hr style='border: none; border-top: 1px solid #E5E5E5; margin: 20px 0;' />
                        <p style='font-size: 12px; color: #888888; text-align: center;'>Hệ thống tuyển dụng DevHub</p>
                    </div>";
                    
                    await _emailHelper.SendEmailAsync(candidateEmail, subject, body);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send evaluation notification for interview {InterviewId}", interview.InterviewId);
            }
        }

        return true;
    }

    public async Task SyncInterviewStatusesAsync()
    {
        var pastScheduledInterviews = await _context.Interviews
            .Where(i => i.Status == "scheduled" && i.ScheduledTime < DateTime.Now)
            .ToListAsync();

        if (pastScheduledInterviews.Any())
        {
            foreach (var interview in pastScheduledInterviews)
            {
                interview.Status = "completed_pending";
                interview.UpdatedAt = DateTime.Now;
            }
            await _context.SaveChangesAsync();
        }
    }
}
