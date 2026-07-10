using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using DevHub.Services.Interfaces;

namespace DevHub.Services.Implementations;

public class ReviewCompanyService : IReviewCompanyService
{
    private readonly IReviewCompanyRepository _reviewCompanyRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IAssignModeratorService _assignModeratorService;
    private readonly INotificationService _notificationService;

    public ReviewCompanyService(
        IReviewCompanyRepository reviewCompanyRepository,
        INotificationRepository notificationRepository,
        IAssignModeratorService assignModeratorService,
        INotificationService notificationService)
    {
        _reviewCompanyRepository = reviewCompanyRepository;
        _notificationRepository = notificationRepository;
        _assignModeratorService = assignModeratorService;
        _notificationService = notificationService;
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

        // Auto assign to least busy moderator
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
