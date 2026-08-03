using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHub.Data;
using DevHub.Models;
using DevHub.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Services.Implementations
{
    public class CompanyFollowerService : ICompanyFollowerService
    {
        private readonly ItrecruitmentDbContext _context;

        public CompanyFollowerService(ItrecruitmentDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ToggleFollowAsync(int candidateId, int companyId)
        {
            var existing = await _context.CompanyFollowers
                .FirstOrDefaultAsync(f => f.CandidateId == candidateId && f.CompanyId == companyId);

            if (existing != null)
            {
                _context.CompanyFollowers.Remove(existing);
                await _context.SaveChangesAsync();
                return false; // Now unfollowed
            }
            else
            {
                var newFollower = new CompanyFollower
                {
                    CandidateId = candidateId,
                    CompanyId = companyId,
                    CreatedAt = DateTime.Now
                };
                _context.CompanyFollowers.Add(newFollower);
                await _context.SaveChangesAsync();
                return true; // Now followed
            }
        }

        public async Task<bool> IsFollowingAsync(int candidateId, int companyId)
        {
            return await _context.CompanyFollowers
                .AnyAsync(f => f.CandidateId == candidateId && f.CompanyId == companyId);
        }

        public async Task<(List<Company> Items, int TotalCount)> GetFollowedCompaniesAsync(int candidateId, string? keyword, int page, int pageSize)
        {
            var query = _context.CompanyFollowers
                .Include(f => f.Company)
                .ThenInclude(c => c.ReviewCompanies)
                .Where(f => f.CandidateId == candidateId)
                .Select(f => f.Company)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var term = keyword.Trim().ToLower();
                query = query.Where(c => c.CompanyName.ToLower().Contains(term));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(c => c.CompanyName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<int> GetFollowedCompaniesCountAsync(int candidateId)
        {
            return await _context.CompanyFollowers.CountAsync(f => f.CandidateId == candidateId);
        }

        public async Task<List<Candidate>> GetFollowersByCompanyAsync(int companyId)
        {
            return await _context.CompanyFollowers
                .Include(f => f.Candidate)
                .ThenInclude(c => c.CandidateNavigation)
                .Where(f => f.CompanyId == companyId)
                .Select(f => f.Candidate)
                .ToListAsync();
        }
    }
}
