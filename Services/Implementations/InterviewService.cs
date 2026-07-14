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

    public InterviewService(ItrecruitmentDbContext context, ILogger<InterviewService> logger, DevHub.Helpers.EmailHelper emailHelper)
    {
        _context = context;
        _logger = logger;
        _emailHelper = emailHelper;
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
            ScheduledTime = scheduledTime,
            InterviewType = interviewType,
            MeetingLink = meetingLink,
            Location = location,
            Notes = notes,
            Status = "SCHEDULED",
            CreatedAt = DateTime.Now
        };

        _context.Interviews.Add(interview);
        await _context.SaveChangesAsync();

        // Create notification for candidate
        try
        {
                
            var title = application?.Job?.Title ?? "công việc";
            
            var n = new Notification
            {
                UserId = application.CandidateId,
                UserType = "CANDIDATE",
                Type = "INTERVIEW",
                Title = "Lịch phỏng vấn được tạo",
                Message = $"Bạn có lịch phỏng vấn vào {scheduledTime:dd/MM/yyyy HH:mm} cho vị trí {title}.",
                ReferenceType = "Interview",
                ReferenceId = interview.InterviewId,
                SeverityLevel = "info",
                IsRead = false,
                CreatedAt = DateTime.Now
            };
            _context.Notifications.Add(n);
            await _context.SaveChangesAsync();

            var emailEnabled = application?.Candidate?.CandidateNavigation?.EmailNotificationsEnabled ?? false;
            var candidateEmail = application?.Candidate?.CandidateNavigation?.Email;
            if (emailEnabled && !string.IsNullOrWhiteSpace(candidateEmail))
            {
                var candidateName = application?.Candidate?.FullName ?? "Bạn";
                var subject = "Thông báo Lịch phỏng vấn mới - DevHub";
                var body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #D6DDEB; border-radius: 8px;'>
                    <h2 style='color: #4640DE;'>Bạn có Lịch phỏng vấn mới</h2>
                    <p>Chào <b>{candidateName}</b>,</p>
                    <p>Nhà tuyển dụng vừa tạo lịch phỏng vấn với bạn cho vị trí <b>{title}</b>.</p>
                    <p>Thời gian: <b>{scheduledTime:dd/MM/yyyy HH:mm}</b></p>
                    <p>Vui lòng đăng nhập vào hệ thống để xem chi tiết (link meeting, địa điểm) và xác nhận tham gia.</p>
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

    public async Task<Interview> UpdateInterviewAsync(int recruiterId, int interviewId, DateTime scheduledTime, string interviewType, string locationOrLink, string? notes)
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

        // Only send notification if the time actually changed
        if (timeChanged)
        {
            try
            {
                var n = new Notification
                {
                    UserId = interview.CandidateId,
                    UserType = "CANDIDATE",
                    Type = "INTERVIEW",
                    Title = "Lịch phỏng vấn bị cập nhật",
                    Message = $"Lịch phỏng vấn của bạn đã được thay đổi thành {scheduledTime:dd/MM/yyyy HH:mm}.",
                    ReferenceType = "Interview",
                    ReferenceId = interview.InterviewId,
                    SeverityLevel = "warning",
                    IsRead = false,
                    CreatedAt = DateTime.Now
                };
                _context.Notifications.Add(n);
                await _context.SaveChangesAsync();

                var emailEnabled = interview.Candidate?.CandidateNavigation?.EmailNotificationsEnabled ?? false;
                var candidateEmail = interview.Candidate?.CandidateNavigation?.Email;
                if (emailEnabled && !string.IsNullOrWhiteSpace(candidateEmail))
                {
                    var title = interview.Application?.Job?.Title ?? "công việc";
                    var candidateName = interview.Candidate?.FullName ?? "Bạn";
                    var subject = "Thay đổi Lịch phỏng vấn - DevHub";
                    var body = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #D6DDEB; border-radius: 8px;'>
                        <h2 style='color: #4640DE;'>Lịch phỏng vấn bị thay đổi</h2>
                        <p>Chào <b>{candidateName}</b>,</p>
                        <p>Lịch phỏng vấn của bạn cho vị trí <b>{title}</b> đã được nhà tuyển dụng cập nhật.</p>
                        <p>Thời gian mới: <b>{scheduledTime:dd/MM/yyyy HH:mm}</b></p>
                        <p>Vui lòng đăng nhập vào hệ thống để xem chi tiết cập nhật.</p>
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
        }

        return interview;
    }
}
