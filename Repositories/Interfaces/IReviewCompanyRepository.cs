using System.Collections.Generic;
using System.Threading.Tasks;
using DevHub.Models;

namespace DevHub.Repositories.Interfaces;

public interface IReviewCompanyRepository
{
    Task<ReviewCompany?> GetByIdAsync(int reviewId);
    Task<(List<ReviewCompany> Items, int TotalCount)> GetPagedAsync(string? status, int page, int pageSize);
    Task<(List<ReviewCompany> Items, int TotalCount)> GetByCandidateAsync(int candidateId, int page, int pageSize);
    Task<ReviewCompany?> GetByCandidateAndCompanyAsync(int candidateId, int companyId);
    Task<int> CreateAsync(ReviewCompany review);
    Task<bool> UpdateAsync(ReviewCompany review);
    Task<bool> UpdateStatusAsync(int reviewId, string status, int moderatorId, string? rejectionReason = null);
    Task UpdateCompanyRatingAsync(int companyId);
}
