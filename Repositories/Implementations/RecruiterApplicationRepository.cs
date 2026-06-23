//AnhPT-18/06/2026
using DevHub.Data;
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using DevHub.ViewModels.Recruiter;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Repositories.Implementations
{
    public class RecruiterApplicationRepository : IRecruiterApplicationRepository
    {
        private readonly ItrecruitmentDbContext _context;

        public RecruiterApplicationRepository(ItrecruitmentDbContext context)
        {
            _context = context;
        }

        public async Task<JobPost?> GetOwnedApprovedJobAsync(int jobId, int recruiterId)
            => await _context.JobPosts
                .AsNoTracking()
                .FirstOrDefaultAsync(j => j.JobId == jobId
                                       && j.RecruiterId == recruiterId
                                       && j.Status != null && j.Status.ToUpper() == "APPROVED");

        // Builds the base query for applications visible to this recruiter, applying all filters.
        private IQueryable<Application> BuildFilteredQuery(int recruiterId, int? jobId, ApplicantFilter filter)
        {
            var q = _context.Applications
                .AsNoTracking()
                .Where(a => a.Job.RecruiterId == recruiterId);

            if (jobId.HasValue)
                q = q.Where(a => a.JobId == jobId.Value);

            // Status (ALL when null/empty).
            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                var st = filter.Status.ToUpper();
                q = q.Where(a => (a.Status ?? "PENDING").ToUpper() == st);
            }

            // Cross-job: filter by job position.
            if (!jobId.HasValue && filter.PositionId.HasValue)
                q = q.Where(a => a.Job.PositionId == filter.PositionId.Value);

            // Tech stack — ANY of the selected techs.
            if (filter.TechIds != null && filter.TechIds.Count > 0)
                q = q.Where(a => a.Candidate.CandidateSkills.Any(s => filter.TechIds.Contains(s.TechId)));

            // Experience bucket (null years treated as 0).
            var bucket = ExperienceBuckets.Find(filter.ExperienceBucket);
            if (bucket != null)
            {
                int min = bucket.Min;
                q = q.Where(a => (a.Candidate.ExperienceYears ?? 0) >= min);
                if (bucket.Max.HasValue)
                {
                    int max = bucket.Max.Value;
                    q = q.Where(a => (a.Candidate.ExperienceYears ?? 0) <= max);
                }
            }

            // Keyword over candidate full name.
            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var kw = filter.Keyword.Trim();
                q = q.Where(a => a.Candidate.FullName.Contains(kw));
            }

            // Preferred location.
            if (!string.IsNullOrWhiteSpace(filter.Location))
            {
                var loc = filter.Location.Trim();
                q = q.Where(a => a.Candidate.PreferredLocation == loc);
            }

            return q;
        }

        public async Task<(List<Application> Items, int TotalCount)> GetApplicantsAsync(
            int recruiterId, int? jobId, ApplicantFilter filter, int page, int pageSize)
        {
            var q = BuildFilteredQuery(recruiterId, jobId, filter);

            var total = await q.CountAsync();

            // Sort.
            q = filter.Sort switch
            {
                "applied_asc" => q.OrderBy(a => a.AppliedAt),
                "exp_desc" => q.OrderByDescending(a => a.Candidate.ExperienceYears ?? 0).ThenByDescending(a => a.AppliedAt),
                "exp_asc" => q.OrderBy(a => a.Candidate.ExperienceYears ?? 0).ThenByDescending(a => a.AppliedAt),
                _ => q.OrderByDescending(a => a.AppliedAt), // applied_desc default
            };

            var items = await q
                .Include(a => a.Candidate).ThenInclude(c => c.CandidateSkills).ThenInclude(s => s.Tech)
                .Include(a => a.Job)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task<(int All, int Pending, int Approved, int Rejected)> CountByStatusAsync(int recruiterId, int? jobId)
        {
            var q = _context.Applications.AsNoTracking().Where(a => a.Job.RecruiterId == recruiterId);
            if (jobId.HasValue)
                q = q.Where(a => a.JobId == jobId.Value);

            var groups = await q
                .GroupBy(a => (a.Status ?? "PENDING").ToUpper())
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            int all = groups.Sum(x => x.Count);
            int pending = groups.Where(x => x.Status == "PENDING").Sum(x => x.Count);
            int approved = groups.Where(x => x.Status == "APPROVED").Sum(x => x.Count);
            int rejected = groups.Where(x => x.Status == "REJECTED").Sum(x => x.Count);
            return (all, pending, approved, rejected);
        }

        public async Task<Application?> GetApplicationDetailAsync(int applicationId, int recruiterId)
            => await _context.Applications
                .AsNoTracking()
                .Include(a => a.Candidate).ThenInclude(c => c.CandidateNavigation)
                .Include(a => a.Candidate).ThenInclude(c => c.CandidateSkills).ThenInclude(s => s.Tech)
                .Include(a => a.Cv)
                .Include(a => a.Job)
                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId && a.Job.RecruiterId == recruiterId);

        public async Task<Application?> UpdateStatusIfPendingAsync(int applicationId, int recruiterId, string newStatus)
        {
            var app = await _context.Applications
                .Include(a => a.Job)
                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId && a.Job.RecruiterId == recruiterId);

            if (app == null) return null;
            if ((app.Status ?? "PENDING").ToUpper() != "PENDING") return null;

            app.Status = newStatus.ToUpper();
            await _context.SaveChangesAsync();
            return app;
        }

        public async Task CreateCandidateNotificationAsync(int candidateId, string title, string message, string severity, int applicationId)
        {
            var n = new Notification
            {
                UserId = candidateId,
                UserType = "CANDIDATE",
                Type = "APPLICATION",
                Title = title,
                Message = message,
                ReferenceType = "Application",
                ReferenceId = applicationId,
                SeverityLevel = severity,
                IsRead = false,
                CreatedAt = DateTime.Now
            };
            _context.Notifications.Add(n);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CommonTechnology>> GetActiveTechOptionsAsync()
            => await _context.CommonTechnologies
                .AsNoTracking()
                .Where(t => t.IsActive == true)
                .OrderBy(t => t.TechName)
                .ToListAsync();

        public async Task<List<CommonJobPosition>> GetActivePositionOptionsAsync()
            => await _context.CommonJobPositions
                .AsNoTracking()
                .Where(p => p.IsActive == true)
                .OrderBy(p => p.PositionName)
                .ToListAsync();

        public async Task<List<string>> GetCandidateLocationOptionsAsync(int recruiterId, int? jobId)
        {
            var q = _context.Applications.AsNoTracking().Where(a => a.Job.RecruiterId == recruiterId);
            if (jobId.HasValue)
                q = q.Where(a => a.JobId == jobId.Value);

            return await q
                .Select(a => a.Candidate.PreferredLocation)
                .Where(l => l != null && l != "")
                .Distinct()
                .OrderBy(l => l)
                .Select(l => l!)
                .ToListAsync();
        }
    }
}
