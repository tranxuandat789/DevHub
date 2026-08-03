//TungVS-06/06/2026
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
        private readonly IArticleRepository _articleRepo;

        public CompanyService(ICompanyRepository companyRepo, IArticleRepository articleRepo)
        {
            _companyRepo = companyRepo;
            _articleRepo = articleRepo;
        }

        public async Task<CompanySearchPageViewModel> SearchCompaniesAsync(CompanySearchInputViewModel input)
        {
            int page = input.Page < 1 ? 1 : input.Page;
            int pageSize = 5; // Updated to 5 companies per page as requested

            // Step 1: Compute global rank dictionary for all visible companies in the system that have AT LEAST 1 review
            var (allRecruiters, _) = await _companyRepo.GetVisibleCompaniesAsync(null, null, null, null, 1, 1000000);
            
            var rankedList = allRecruiters
                .Where(r => r.ReviewCompanies.Any()) // ONLY companies with reviews get a rank
                .Select(r => new {
                    CompanyId = r.CompanyId,
                    Rating = r.AverageRating ?? 0m,
                    LatestReviewDate = r.ReviewCompanies.Max(rev => rev.CreatedAt ?? DateTime.MinValue),
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
            List<DevHub.Models.Company> items;
            int totalCount;

            if (input.TopN.HasValue)
            {
                // If Top N is selected, we must apply search criteria to ALL recruiters, 
                // then filter only those that have a rank, sort them by rank, and take N.
                var (filteredRecruiters, _) = await _companyRepo.GetVisibleCompaniesAsync(
                    input.SearchTerm, 
                    input.SelectedTechs, 
                    input.SelectedPositions, 
                    input.SortOrder,
                    1, 1000000);

                var rankedFiltered = filteredRecruiters
                    .Where(r => ranksDict.ContainsKey(r.CompanyId))
                    .OrderBy(r => ranksDict[r.CompanyId])
                    .Take(input.TopN.Value)
                    .ToList();

                totalCount = rankedFiltered.Count;
                items = rankedFiltered.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            }
            else
            {
                var result = await _companyRepo.GetVisibleCompaniesAsync(
                    input.SearchTerm, 
                    input.SelectedTechs, 
                    input.SelectedPositions, 
                    input.SortOrder,
                    page, 
                    pageSize);
                items = result.Items;
                totalCount = result.TotalCount;
            }

            var list = items.Select(r =>
            {
                double? avgRating = null;
                int totalReviews = r.ReviewCompanies.Count;
                if (totalReviews > 0)
                {
                    avgRating = (double)r.ReviewCompanies.Sum(rev => rev.Rating) / totalReviews;
                }

                // Resolve system rank from the pre-computed global rankings dictionary
                int? systemRank = ranksDict.TryGetValue(r.CompanyId, out int rk) ? rk : null;

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

            // Populate JobCount, TechStacks and ArticlePreviews for each company
            foreach (var company in list)
            {
                var jobs = await _companyRepo.GetCompanyJobsAsync(company.CompanyId);
                company.JobCount = jobs.Count;
                company.TechStacks = jobs.SelectMany(j => j.Teches.Select(t => t.TechName)).Distinct().ToList();

                var articles = await _articleRepo.GetArticlesByCompanyAsync(company.CompanyId);
                var published = articles.Where(a => a.Status == "APPROVED").ToList();
                company.TotalArticleCount = published.Count;
                company.ArticlesPreviews = published.Take(2)
                    .Select(a => (a.ArticleId, a.Title ?? "(Không có tiêu đề)", a.Slug))
                    .ToList();
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
                TopN = input.TopN,
                AvailableTechs = availableTechs,
                AvailablePositions = availablePositions,
                SelectedTechs = input.SelectedTechs ?? new(),
                SelectedPositions = input.SelectedPositions ?? new()
            };
        }

        public async Task<CompanyDetailsViewModel?> GetCompanyDetailsAsync(int id, int? currentCandidateId = null)
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

            var articles = await _articleRepo.GetArticlesByCompanyAsync(id);
            var publishedArticles = articles.Where(a => a.Status == "APPROVED").ToList();

            var vm = new CompanyDetailsViewModel
            {
                Company = company,
                ActiveJobs = jobs,
                Articles = publishedArticles,
                AverageRating = avgRating,
                TotalReviews = totalReviews
            };
            
            if (currentCandidateId.HasValue)
            {
                var userReview = company.ReviewCompanies.FirstOrDefault(r => r.CandidateId == currentCandidateId.Value);
                if (userReview != null)
                {
                    vm.HasUserReviewed = true;
                    vm.UserReviewId = userReview.ReviewId;
                }
            }

            return vm;
        }
    }
}
