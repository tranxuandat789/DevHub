using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHub.Repositories.Interfaces;
using DevHub.Services.Interfaces;
using DevHub.ViewModels.Company;

namespace DevHub.Services.Implementations
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepo;

        public CompanyService(ICompanyRepository companyRepo)
        {
            _companyRepo = companyRepo;
        }

        public async Task<CompanySearchPageViewModel> SearchCompaniesAsync(CompanySearchInputViewModel input)
        {
            int page = input.Page < 1 ? 1 : input.Page;
            int pageSize = 5; // Updated to 5 companies per page as requested

            // Step 1: Compute global rank dictionary for all visible companies in the system
            // Visible companies are those with ProfileCompletion >= 70%
            var (allRecruiters, _) = await _companyRepo.GetVisibleCompaniesAsync(null, null, null, null, 1, 1000000);
            
            var rankedList = allRecruiters
                .Select(r => new {
                    CompanyId = r.CompanyId,
                    Rating = r.AverageRating ?? 0m,
                    LatestReviewDate = r.ReviewCompanies.Any() 
                        ? r.ReviewCompanies.Max(rev => rev.CreatedAt ?? DateTime.MinValue) 
                        : DateTime.MinValue,
                    CompanyName = r.CompanyName
                })
                .OrderByDescending(x => x.Rating)
                .ThenByDescending(x => x.LatestReviewDate)
                .ThenBy(x => x.CompanyName)
                .ToList();

            var ranksDict = rankedList
                .Select((x, idx) => new { x.CompanyId, Rank = idx + 1 })
                .ToDictionary(x => x.CompanyId, x => x.Rank);

            // Step 2: Fetch filtered & sorted recruiters for the current search/filter criteria
            var (items, totalCount) = await _companyRepo.GetVisibleCompaniesAsync(
                input.SearchTerm, 
                input.SelectedTechs, 
                input.SelectedPositions, 
                input.SortOrder,
                page, 
                pageSize);

            var list = items.Select(r =>
            {
                double? avgRating = null;
                int totalReviews = r.ReviewCompanies.Count;
                if (totalReviews > 0)
                {
                    avgRating = (double)r.ReviewCompanies.Sum(rev => rev.Rating) / totalReviews;
                }

                // Resolve system rank from the pre-computed global rankings dictionary
                int systemRank = ranksDict.TryGetValue(r.CompanyId, out int rk) ? rk : 9999;

                return new CompanySearchItemViewModel
                {
                    CompanyId = r.CompanyId,
                    CompanyName = r.CompanyName,
                    CompanyLogoUrl = r.CompanyLogoUrl,
                    CompanyAddress = r.CompanyAddress,
                    Industry = r.Industry,
                    AverageRating = avgRating,
                    TotalReviews = totalReviews,
                    SystemRank = systemRank
                };
            }).ToList();

            // Populate JobCount and TechStacks badges for each company
            foreach (var company in list)
            {
                var jobs = await _companyRepo.GetCompanyJobsAsync(company.CompanyId);
                company.JobCount = jobs.Count;
                company.TechStacks = jobs.SelectMany(j => j.Teches.Select(t => t.TechName)).Distinct().ToList();
            }

            // Fetch filter list options (Tech stacks and ALL positions from DB)
            var availableTechs = await _companyRepo.GetActiveTechnologiesAsync();
            var availablePositions = await _companyRepo.GetActiveJobPositionsAsync();

            return new CompanySearchPageViewModel
            {
                Companies = list,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                Name = null,
                Industry = null,
                Address = null,
                SearchTerm = input.SearchTerm,
                SortOrder = input.SortOrder,
                AvailableTechs = availableTechs,
                AvailablePositions = availablePositions,
                SelectedTechs = input.SelectedTechs ?? new(),
                SelectedPositions = input.SelectedPositions ?? new()
            };
        }

        public async Task<CompanyDetailsViewModel?> GetCompanyDetailsAsync(int id)
        {
            var company = await _companyRepo.GetCompanyDetailsAsync(id);
            if (company == null || (company.ProfileCompletion ?? 0) < 70)
                return null;

            var jobs = await _companyRepo.GetCompanyJobsAsync(id);

            double? avgRating = null;
            int totalReviews = company.ReviewCompanies.Count;
            if (totalReviews > 0)
            {
                avgRating = (double)company.ReviewCompanies.Sum(rev => rev.Rating) / totalReviews;
            }

            return new CompanyDetailsViewModel
            {
                Company = company,
                ActiveJobs = jobs,
                AverageRating = avgRating,
                TotalReviews = totalReviews
            };
        }
    }
}
