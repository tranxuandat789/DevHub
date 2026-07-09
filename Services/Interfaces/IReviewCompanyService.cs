using System.Collections.Generic;
using System.Threading.Tasks;
using DevHub.Models;

namespace DevHub.Services.Interfaces;

public interface IReviewCompanyService
{
    Task<ReviewCompany?> GetByIdAsync(int reviewId);
    Task<(List<ReviewCompany> Items, int TotalCount)> GetPagedForModeratorAsync(string? status, int page, int pageSize);
    Task<(List<ReviewCompany> Items, int TotalCount)> GetByCandidateAsync(int candidateId, int page, int pageSize);
    Task<bool> HasReviewedAsync(int candidateId, int companyId);
    Task<ReviewCompany?> GetByCandidateAndCompanyAsync(int candidateId, int companyId);
    Task<(bool Success, string Message)> CreateReviewAsync(ReviewCompany review);
    Task<(bool Success, string Message)> UpdateReviewAsync(ReviewCompany review, int candidateId);
    Task<bool> ApproveReviewAsync(int reviewId, int moderatorId);
    Task<bool> RejectReviewAsync(int reviewId, int moderatorId, string reason);
}
