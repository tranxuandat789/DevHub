using System;
using DevHub.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevHub.Repositories.Interfaces;

public interface IJobPostRepository
{
    Task<List<JobPost>> GetPendingJobPostsAsync(DateTime? fromDate, DateTime? toDate, string? sortOrder);
    Task<JobPost?> GetJobPostByIdAsync(int id);
    Task UpdateJobPostAsync(JobPost job);
}
