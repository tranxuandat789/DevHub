using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using DevHub.Services.Interfaces;
using DevHub.Helpers;

namespace DevHub.Services.Implementations;

public class ReviewCompanyService : IReviewCompanyService
{
    private readonly IReviewCompanyRepository _reviewCompanyRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IAssignModeratorService _assignModeratorService;
    private readonly INotificationService _notificationService;
    private readonly EmailHelper _emailHelper;
    private readonly IUserAccountRepository _userRepository;

    public ReviewCompanyService(
        IReviewCompanyRepository reviewCompanyRepository,
        INotificationRepository notificationRepository,
        IAssignModeratorService assignModeratorService,
        INotificationService notificationService,
        EmailHelper emailHelper,
        IUserAccountRepository userRepository)
    {
        _reviewCompanyRepository = reviewCompanyRepository;
        _notificationRepository = notificationRepository;
        _assignModeratorService = assignModeratorService;
        _notificationService = notificationService;
        _emailHelper = emailHelper;
        _userRepository = userRepository;
    }

    public async Task<ReviewCompany?> GetByIdAsync(int reviewId)
    {
        return await _reviewCompanyRepository.GetByIdAsync(reviewId);
    }

    public async Task<(List<ReviewCompany> Items, int TotalCount)> GetPagedForModeratorAsync(string? status, int page, int pageSize)
    {
        return await _reviewCompanyRepository.GetPagedAsync(status, page, pageSize);
    }

    public async Task<(List<ReviewCompany> Items, int TotalCount)> GetByCandidateAsync(int candidateId, int page, int pageSize)
    {
        return await _reviewCompanyRepository.GetByCandidateAsync(candidateId, page, pageSize);
    }

    public async Task<bool> HasReviewedAsync(int candidateId, int companyId)
    {
        var existing = await _reviewCompanyRepository.GetByCandidateAndCompanyAsync(candidateId, companyId);
        return existing != null;
    }

    public async Task<ReviewCompany?> GetByCandidateAndCompanyAsync(int candidateId, int companyId)
    {
        return await _reviewCompanyRepository.GetByCandidateAndCompanyAsync(candidateId, companyId);
    }

    public async Task<(bool Success, string Message)> CreateReviewAsync(ReviewCompany review)
    {
        var existing = await _reviewCompanyRepository.GetByCandidateAndCompanyAsync(review.CandidateId, review.CompanyId);
        if (existing != null)
        {
            return (false, "Bạn đã đánh giá công ty này rồi.");
        }

        await _reviewCompanyRepository.CreateAsync(review);

        // Auto-assign moderator cho review mới
        var assignedModId = await _assignModeratorService.AutoAssignNewRecordAsync("REVIEW", review.ReviewId);
        
        // Notify the assigned moderator
        if (assignedModId.HasValue)
        {
            var company = await _reviewCompanyRepository.GetByIdAsync(review.ReviewId); // to get company name
            await _notificationService.SendNotificationAsync(
                userId: assignedModId.Value,
                userType: "MODERATOR",
                title: "Đánh giá công ty mới chờ duyệt",
                message: $"Một đánh giá mới cho công ty '{company?.Company?.CompanyName ?? "Unknown"}' đang chờ bạn duyệt.",
                type: "REVIEW",
                severity: "info",
                referenceId: review.ReviewId,
                referenceType: "Review"
            );

            var modAccount = await _userRepository.GetByIdAsync(assignedModId.Value);
            if (modAccount != null && modAccount.EmailNotificationsEnabled && !string.IsNullOrEmpty(modAccount.Email))
            {
                string subject = "Bạn có công việc mới cần xử lý - DevHub";
                string content = $@"
                    <p>Chào <strong>bạn</strong>,</p>
                    <p>Bạn vừa được gán <strong>1 Đánh giá công ty</strong> mới cần xét duyệt trên hệ thống DevHub.</p>
                    <p>Vui lòng đăng nhập vào hệ thống quản trị để kiểm tra và xử lý kịp thời.</p>";
                string body = DevHub.Helpers.EmailHelper.GetBaseTemplate("Công Việc Mới Trên DevHub", content);
                await _emailHelper.SendEmailAsync(modAccount.Email, subject, body);
            }
        }

        return (true, "Gửi đánh giá thành công. Vui lòng chờ kiểm duyệt.");
    }

    public async Task<(bool Success, string Message)> UpdateReviewAsync(ReviewCompany review, int candidateId)
    {
        var existing = await _reviewCompanyRepository.GetByIdAsync(review.ReviewId);
        if (existing == null) return (false, "Không tìm thấy đánh giá.");
        
        if (existing.CandidateId != candidateId) return (false, "Bạn không có quyền sửa đánh giá này.");
        
        if (existing.Status == "APPROVED") return (false, "Không thể sửa đánh giá đã được duyệt.");

        var success = await _reviewCompanyRepository.UpdateAsync(review);
        if (success)
        {
            if (existing.ModeratorId.HasValue)
            {
                await _notificationService.SendNotificationAsync(
                    userId: existing.ModeratorId.Value,
                    userType: "MODERATOR",
                    title: "Đánh giá cập nhật chờ duyệt",
                    message: $"Đánh giá ID {existing.ReviewId} đã được cập nhật và cần duyệt lại.",
                    type: "REVIEW",
                    severity: "info",
                    referenceId: existing.ReviewId,
                    referenceType: "Review"
                );

                var modAccount = await _userRepository.GetByIdAsync(existing.ModeratorId.Value);
                if (modAccount != null && modAccount.EmailNotificationsEnabled && !string.IsNullOrEmpty(modAccount.Email))
                {
                    string subject = "Bạn có công việc cập nhật cần xử lý - DevHub";
                    string content = $@"
                        <p>Chào <strong>bạn</strong>,</p>
                        <p>Bạn vừa được gán <strong>1 Đánh giá công ty (cập nhật)</strong> cần xét duyệt lại trên hệ thống DevHub.</p>
                        <p>Vui lòng đăng nhập vào hệ thống quản trị để kiểm tra và xử lý kịp thời.</p>";
                    string body = DevHub.Helpers.EmailHelper.GetBaseTemplate("Công Việc Mới Trên DevHub", content);
                    await _emailHelper.SendEmailAsync(modAccount.Email, subject, body);
                }
            }
            return (true, "Cập nhật đánh giá thành công. Vui lòng chờ kiểm duyệt lại.");
        }
        return (false, "Có lỗi xảy ra khi cập nhật đánh giá.");
    }

    public async Task<bool> ApproveReviewAsync(int reviewId, int moderatorId)
    {
        var success = await _reviewCompanyRepository.UpdateStatusAsync(reviewId, "APPROVED", moderatorId);
        if (success)
        {
            var review = await _reviewCompanyRepository.GetByIdAsync(reviewId);
            if (review != null)
            {
                // Cập nhật rating công ty
                await _reviewCompanyRepository.UpdateCompanyRatingAsync(review.CompanyId);

                // Gửi notification cho Candidate
                await _notificationService.SendNotificationAsync(
                    userId: review.CandidateId,
                    userType: "CANDIDATE",
                    title: "Đánh giá của bạn đã được duyệt",
                    message: $"Đánh giá của bạn cho công ty {review.Company?.CompanyName ?? ""} đã được duyệt và hiển thị công khai.",
                    type: "REVIEW_APPROVED",
                    severity: "info",
                    referenceId: reviewId,
                    referenceType: "Review"
                );

                // Gửi email cho Candidate
                if (review.Candidate?.CandidateNavigation?.Email != null && 
                    review.Candidate.CandidateNavigation.EmailNotificationsEnabled)
                {
                    string emailBody = EmailHelper.GetBaseTemplate(
                        "Đánh giá công ty đã được duyệt",
                        $"<p>Chào {review.Candidate.FullName},</p><p>Đánh giá của bạn cho công ty <b>{review.Company?.CompanyName ?? ""}</b> đã được duyệt và hiện đang hiển thị công khai trên hệ thống.</p><p>Cảm ơn bạn đã đóng góp đánh giá!</p>"
                    );
                    await _emailHelper.SendEmailAsync(review.Candidate.CandidateNavigation.Email, "Đánh giá của bạn đã được duyệt", emailBody);
                }
            }
        }
        return success;
    }

    public async Task<bool> RejectReviewAsync(int reviewId, int moderatorId, string reason)
    {
        var success = await _reviewCompanyRepository.UpdateStatusAsync(reviewId, "REJECTED", moderatorId, reason);
        if (success)
        {
            var review = await _reviewCompanyRepository.GetByIdAsync(reviewId);
            if (review != null)
            {
                // Remove from company rating
                await _reviewCompanyRepository.UpdateCompanyRatingAsync(review.CompanyId);

                // Gửi notification cho Candidate
                await _notificationService.SendNotificationAsync(
                    userId: review.CandidateId,
                    userType: "CANDIDATE",
                    title: "Đánh giá của bạn đã bị từ chối",
                    message: $"Đánh giá của bạn cho công ty {review.Company?.CompanyName ?? ""} đã bị từ chối với lý do: {reason}",
                    type: "REVIEW_REJECTED",
                    severity: "warning",
                    referenceId: reviewId,
                    referenceType: "Review"
                );

                // Gửi email cho Candidate
                if (review.Candidate?.CandidateNavigation?.Email != null && 
                    review.Candidate.CandidateNavigation.EmailNotificationsEnabled)
                {
                    string emailBody = EmailHelper.GetBaseTemplate(
                        "Đánh giá công ty bị từ chối",
                        $"<p>Chào {review.Candidate.FullName},</p><p>Đánh giá của bạn cho công ty <b>{review.Company?.CompanyName ?? ""}</b> đã bị từ chối.</p><p><b>Lý do:</b> {reason}</p><p>Vui lòng xem xét lại nội dung và tuân thủ các quy định của hệ thống.</p>"
                    );
                    await _emailHelper.SendEmailAsync(review.Candidate.CandidateNavigation.Email, "Đánh giá của bạn bị từ chối", emailBody);
                }
            }
        }
        return success;
    }

    private int GetUserIdByCandidateId(int candidateId)
    {
        // In DevHub, candidate_id IS the user_id
        return candidateId;
    }
}
