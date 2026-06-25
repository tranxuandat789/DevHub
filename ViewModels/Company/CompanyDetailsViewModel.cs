using System.Collections.Generic;
using DevHub.Models;

namespace DevHub.ViewModels.Company
{
    public class CompanyDetailsViewModel
    {
        public DevHub.Models.Recruiter Recruiter { get; set; } = null!;
        public List<JobPost> ActiveJobs { get; set; } = new();
        public double? AverageRating { get; set; }
        public int TotalReviews { get; set; }
    }
}
