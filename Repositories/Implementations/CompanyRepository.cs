using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHub.Data;
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Repositories.Implementations
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ItrecruitmentDbContext _context;

        public CompanyRepository(ItrecruitmentDbContext context)
        {
            _context = context;
        }

        public async Task<(List<Company> Items, int TotalCount)> GetVisibleCompaniesAsync(
            string? searchTerm, 
            List<int>? selectedTechs, 
            List<int>? selectedPositions, 
            string? sortOrder,
            int page, 
            int pageSize)
        {
            var query = _context.Companies
                .Include(r => r.ReviewCompanies)
                .Where(r => r.ProfileCompletion >= 70);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();
                query = query.Where(r => r.CompanyName.Contains(term) || (r.CompanyAddress != null && r.CompanyAddress.Contains(term)));
            }

            if (selectedTechs != null && selectedTechs.Any())
            {
                query = query.Where(r => _context.JobPosts.Any(j => 
                    j.CompanyId == r.CompanyId && 
                    j.Status == "APPROVED" && 
                    j.Teches.Any(t => selectedTechs.Contains(t.TechId))));
            }

            if (selectedPositions != null && selectedPositions.Any())
            {
                query = query.Where(r => _context.JobPosts.Any(j => 
                    j.CompanyId == r.CompanyId && 
                    j.Status == "APPROVED" && 
                    selectedPositions.Contains(j.PositionId)));
            }

            var totalCount = await query.CountAsync();

            // Load items from database to perform safe sorting with tie-breaker details
            var items = await query.ToListAsync();

            // Sort in-memory to prevent EF Core translation errors for complex aggregations (Max review date)
            if (sortOrder == "rating_asc")
            {
                // Công ty chưa có review (0 sao) → ĐẦU danh sách
                items = items
                    .OrderBy(r => r.TotalReviews > 0 ? 1 : 0)          // 0-review lên đầu
                    .ThenBy(r => r.AverageRating ?? 0m)
                    .ThenBy(r => r.ReviewCompanies.Any()
                        ? r.ReviewCompanies.Max(rev => rev.CreatedAt ?? DateTime.MinValue)
                        : DateTime.MinValue)
                    .ThenBy(r => r.CompanyName)
                    .ToList();
            }
            else
            {
                // Default: rating_desc — Công ty chưa có review (0 sao) → CUỐI danh sách
                items = items
                    .OrderBy(r => r.TotalReviews > 0 ? 0 : 1)          // 0-review xuống cuối
                    .ThenByDescending(r => r.AverageRating ?? 0m)
                    .ThenByDescending(r => r.ReviewCompanies.Any()
                        ? r.ReviewCompanies.Max(rev => rev.CreatedAt ?? DateTime.MinValue)
                        : DateTime.MinValue)
                    .ThenBy(r => r.CompanyName)
                    .ToList();
            }


            return (items, totalCount);
        }

        public async Task<Company?> GetCompanyDetailsAsync(int companyId)
        {
            return await _context.Companies
                .Include(r => r.Recruiters) // Includes UserAccount for email contact
                .Include(r => r.ReviewCompanies)
                    .ThenInclude(rev => rev.Candidate)
                .FirstOrDefaultAsync(r => r.CompanyId == companyId);
        }

        public async Task<List<JobPost>> GetCompanyJobsAsync(int companyId)
        {
            return await _context.JobPosts
                .Include(j => j.Teches) // Includes Tech stack badges for Job cards
                .Include(j => j.Provinces)
                .Where(j => j.CompanyId == companyId && j.Status == "APPROVED")
                .ToListAsync();
        }

        public async Task<List<CommonTechnology>> GetActiveTechnologiesAsync()
        {
            return await _context.CommonTechnologies
                .Where(t => t.IsActive == true)
                .OrderBy(t => t.TechName)
                .ToListAsync();
        }

        public async Task<List<CommonJobPosition>> GetActiveJobPositionsAsync()
        {
            return await _context.CommonJobPositions
                .Where(p => p.IsActive == true)
                .OrderBy(p => p.PositionName)
                .ToListAsync();
        }

        public async Task<Company> AddCompanyAsync(Company company)
        {
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();
            return company;
        }
    }
}
