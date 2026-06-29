using System.Collections.Generic;

namespace DevHub.Models
{
    public class HomeViewModel
    {
        public List<JobPost> FeaturedJobs { get; set; } = new List<JobPost>();
        public List<FeaturedCompanyViewModel> FeaturedCompanies { get; set; } = new List<FeaturedCompanyViewModel>();
        public List<BlogPost> FeaturedBlogs { get; set; } = new List<BlogPost>();
        public HashSet<int> BookmarkedJobIds { get; set; } = new HashSet<int>();
    }

    public class FeaturedCompanyViewModel
    {
        public int RecruiterId { get; set; }
        public string CompanyName { get; set; } = null!;
        public string? CompanyLogoUrl { get; set; }
        public int JobCount { get; set; }
    }
}
