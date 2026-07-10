using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHub.Data;
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Repositories.Implementations;

public class ReviewCompanyRepository : IReviewCompanyRepository
{
    private readonly ItrecruitmentDbContext _db;

    public ReviewCompanyRepository(ItrecruitmentDbContext db)
    {
        _db = db;
    }

    public async Task<ReviewCompany?> GetByIdAsync(int reviewId)
    {
        return await _db.ReviewCompanies
            .Include(r => r.Candidate)
                .ThenInclude(c => c.CandidateNavigation)
            .Include(r => r.Company)
            .FirstOrDefaultAsync(r => r.ReviewId == reviewId);
    }

    public async Task<(List<ReviewCompany> Items, int TotalCount)> GetPagedAsync(string? status, int page, int pageSize)
    {
        var query = _db.ReviewCompanies
            .Include(r => r.Candidate)
            .Include(r => r.Company)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(r => r.Status == status);
        }

        int totalCount = await query.CountAsync();
        
        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<(List<ReviewCompany> Items, int TotalCount)> GetByCandidateAsync(int candidateId, int page, int pageSize)
    {
        var query = _db.ReviewCompanies
            .Include(r => r.Company)
            .Where(r => r.CandidateId == candidateId);

        int totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<ReviewCompany?> GetByCandidateAndCompanyAsync(int candidateId, int companyId)
    {
        return await _db.ReviewCompanies
            .FirstOrDefaultAsync(r => r.CandidateId == candidateId && r.CompanyId == companyId);
    }

    public async Task<int> CreateAsync(ReviewCompany review)
    {
        review.CreatedAt = DateTime.Now;
        review.UpdatedAt = DateTime.Now;
        review.Status = "PENDING";
        _db.ReviewCompanies.Add(review);
        await _db.SaveChangesAsync();
        return review.ReviewId;
    }

    public async Task<bool> UpdateAsync(ReviewCompany review)
    {
        var existing = await _db.ReviewCompanies.FindAsync(review.ReviewId);
        if (existing == null) return false;

        existing.Rating = review.Rating;
        existing.SalaryRating = review.SalaryRating;
        existing.TrainingRating = review.TrainingRating;
        existing.CareRating = review.CareRating;
        existing.CultureRating = review.CultureRating;
        existing.WorkspaceRating = review.WorkspaceRating;
        existing.OtPolicy = review.OtPolicy;
        existing.Recommend = review.Recommend;
        existing.Pros = review.Pros;
        existing.Cons = review.Cons;
        
        // If it was rejected and user edited it, we reset status to pending
        if (existing.Status == "REJECTED" || existing.Status == "PENDING")
        {
            existing.Status = "PENDING";
            existing.RejectionReason = null;
        }

        existing.UpdatedAt = DateTime.Now;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateStatusAsync(int reviewId, string status, int moderatorId, string? rejectionReason = null)
    {
        var review = await _db.ReviewCompanies.FindAsync(reviewId);
        if (review == null) return false;

        review.Status = status;
        review.ModeratorId = moderatorId;
        review.ModeratedAt = DateTime.Now;
        review.RejectionReason = rejectionReason;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task UpdateCompanyRatingAsync(int companyId)
    {
        var company = await _db.Companies.FindAsync(companyId);
        if (company == null) return;

        var approvedReviews = await _db.ReviewCompanies
            .Where(r => r.CompanyId == companyId && r.Status == "APPROVED")
            .ToListAsync();

        if (approvedReviews.Any())
        {
            company.TotalReviews = approvedReviews.Count;
            company.AverageRating = (decimal)approvedReviews.Average(r => r.Rating);
        }
        else
        {
            company.TotalReviews = 0;
            company.AverageRating = 0;
        }

        await _db.SaveChangesAsync();
    }

}
