using System;
using DevHub.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevHub.Repositories.Interfaces;

public interface IJobPostRepository
{
    Task<(List<JobPost> Items, int TotalCount)> GetPendingJobPostsAsync(DateTime? fromDate, DateTime? toDate, string? sortOrder, int page, int pageSize);
    Task<JobPost?> GetJobPostByIdAsync(int id);
    Task UpdateJobPostAsync(JobPost job);
}
