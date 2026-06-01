using DevHub.Models;
using DevHub.Repositories.Interfaces;
using DevHub.Services.Interfaces;
using DevHub.ViewModels.Recruiter;
namespace DevHub.Services.Implementations
{
    public class RecruiterDashboardService : IRecruiterDashboardService
    {
        private readonly IRecruiterDashboardRepository _repo;

        public RecruiterDashboardService(IRecruiterDashboardRepository repo)
        {
            _repo = repo;
        }

        public async Task<RecruiterDashboardViewModel> GetDashboardAsync(int recruiterId)
        {
            var jobPosts   = await _repo.GetJobPostsAsync(recruiterId);
            var interviews = await _repo.GetInterviewsAsync(recruiterId);

            var scheduled = interviews
                .Where(i => i.Status == "Scheduled" || i.Status == "Pending")
                .ToList();

            var completed = interviews
                .Where(i => i.Status == "Completed" || i.Status == "Done")
                .ToList();

            return new RecruiterDashboardViewModel
            {
                TotalJobPosts              = jobPosts.Count,
                TotalApplications          = jobPosts.Sum(j => j.ApplicationCount ?? 0),
                TotalScheduledInterviews   = scheduled.Count,
                TotalCompletedInterviews   = completed.Count,

                JobPostApplicantCounts = jobPosts.Select(j => new JobPostSummaryViewModel
                {
                    Title          = j.Title,
                    CreatedAt      = j.CreatedAt,
                    ApplicantCount = j.ApplicationCount ?? 0,
                    Status         = j.Status
                }).ToList(),

                ScheduledInterviews = scheduled.Select(i => new InterviewSummaryViewModel
                {
                    InterviewId        = i.InterviewId,
                    ScheduledTime      = i.ScheduledTime,
                    CandidateFullName  = i.Candidate?.FullName,
                    JobTitle           = i.Application?.Job?.Title,
                    MeetingLink        = i.MeetingLink,
                    Status             = i.Status
                }).ToList(),

                CompletedInterviews = completed.Select(i => new InterviewSummaryViewModel
                {
                    InterviewId        = i.InterviewId,
                    ScheduledTime      = i.ScheduledTime,
                    CandidateFullName  = i.Candidate?.FullName,
                    JobTitle           = i.Application?.Job?.Title,
                    MeetingLink        = i.MeetingLink,
                    Status             = i.Status
                }).ToList()
            };
        }
    }
}