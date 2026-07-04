using System;
using DevHub.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevHub.Repositories.Interfaces;

public interface IJobPostRepository
{
    Task<(List<JobPost> Items, int TotalCount)> GetPendingJobPostsAsync(int moderatorId, DateTime? fromDate, DateTime? toDate, string? sortOrder, int page, int pageSize);
    Task<JobPost?> GetJobPostByIdAsync(int id);
    Task UpdateJobPostAsync(JobPost job);

    // After a moderator re-approves an edited job, notify candidates with an active (PENDING/APPROVED)
    // application that the posting is live again (their application was frozen during re-review).
    Task NotifyApplicantsOnJobReApprovedAsync(int jobId, string jobTitle);
}
