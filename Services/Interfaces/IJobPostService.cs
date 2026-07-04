using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevHub.Models;

namespace DevHub.Services.Interfaces;

public interface IJobPostService
{
    Task<(List<JobPost> Items, int TotalCount)> GetPendingJobsAsync(int moderatorId, DateTime? fromDate, DateTime? toDate, string? sortOrder, int page, int pageSize);
    Task<JobPost?> GetJobPostByIdAsync(int jobId);
    Task<bool> ApproveJobAsync(int jobId, int moderatorId);
    Task<bool> RejectJobAsync(int jobId, int moderatorId, string reason);
}
