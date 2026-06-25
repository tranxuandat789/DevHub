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

        public async Task<JobPost?> GetOwnedJobAsync(int jobId, int recruiterId)
            => await _context.JobPosts
                .AsNoTracking()
                .FirstOrDefaultAsync(j => j.JobId == jobId
                                       && j.RecruiterId == recruiterId
                                       // The recruiter may VIEW the applicant list for any non-rejected job, including a
                                       // job that just went back to PENDING (re-review). The list is read-only while
                                       // PENDING — approve/reject is gated separately. Only REJECTED jobs are blocked.
                                       && j.Status != null
                                       && j.Status.ToUpper() != "REJECTED");

        // Builds the base query for applications visible to this recruiter, applying all filters.
        // includeStatus = false => skip the status filter (used by CountByStatusAsync so each tab
        // count reflects the other active filters but not the status itself).
        private IQueryable<Application> BuildFilteredQuery(int recruiterId, int? jobId, ApplicantFilter filter, bool includeStatus = true)
        {
            var q = _context.Applications
                .AsNoTracking()
                .Where(a => a.Job.RecruiterId == recruiterId);

            if (jobId.HasValue)
                q = q.Where(a => a.JobId == jobId.Value);

            // Status (ALL when null/empty). Skipped for tab-count queries.
            // A tab key maps to a GROUP of raw statuses (e.g. "APPROVED" tab = APPROVED + FINISHED,
            // both displayed as "Đã duyệt"), so a status tab matches every status under that label.
            if (includeStatus && !string.IsNullOrWhiteSpace(filter.Status))
            {
                var statuses = MapStatusGroup(filter.Status);
                q = q.Where(a => statuses.Contains((a.Status ?? "PENDING").ToUpper()));
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

        // Maps a status-tab key to the set of raw application statuses it represents.
        private static string[] MapStatusGroup(string tab) => tab.ToUpper() switch
        {
            "APPROVED" => new[] { "APPROVED", "FINISHED" }, // "Đã duyệt"
            "HIRED" => new[] { "HIRED" },                   // "Trúng tuyển"
            "PENDING" => new[] { "PENDING" },
            "REJECTED" => new[] { "REJECTED" },
            var s => new[] { s }                            // any other exact status
        };

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

        public async Task<(int All, int Pending, int Approved, int Hired, int Rejected)> CountByStatusAsync(int recruiterId, int? jobId, ApplicantFilter filter)
        {
            // Reuse the same filters as the list (tech/experience/keyword/location/position) but ignore
            // the status filter, so each tab shows how many results exist per status under the current filters.
            var q = BuildFilteredQuery(recruiterId, jobId, filter, includeStatus: false);

            var groups = await q
                .GroupBy(a => (a.Status ?? "PENDING").ToUpper())
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            int all = groups.Sum(x => x.Count);
            int pending = groups.Where(x => x.Status == "PENDING").Sum(x => x.Count);
            // "Đã duyệt" tab groups APPROVED + FINISHED.
            int approved = groups.Where(x => x.Status == "APPROVED" || x.Status == "FINISHED").Sum(x => x.Count);
            int hired = groups.Where(x => x.Status == "HIRED").Sum(x => x.Count);
            int rejected = groups.Where(x => x.Status == "REJECTED").Sum(x => x.Count);
            return (all, pending, approved, hired, rejected);
        }

        public async Task<Application?> GetApplicationDetailAsync(int applicationId, int recruiterId)
            => await _context.Applications
                .AsNoTracking()
                .Include(a => a.Candidate).ThenInclude(c => c.CandidateNavigation)
                .Include(a => a.Candidate).ThenInclude(c => c.CandidateSkills).ThenInclude(s => s.Tech)
                .Include(a => a.Cv)
                .Include(a => a.Job)
                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId && a.Job.RecruiterId == recruiterId);

        public async Task<(Application? App, string? JobStatus)> GetApplicationWithJobStatusAsync(int applicationId, int recruiterId)
        {
            var app = await _context.Applications
                .AsNoTracking()
                .Include(a => a.Job)
                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId && a.Job.RecruiterId == recruiterId);

            return (app, app?.Job?.Status);
        }

        public async Task<Application?> UpdateStatusIfPendingAsync(int applicationId, int recruiterId, string newStatus)
        {
            var status = newStatus.ToUpper();

            // Atomic guard against a race (e.g. two recruiters on a shared account approving at once)
            // The job-frozen guard (a.Job.Status != PENDING) is also enforced here so an approve/reject
            // can never change if the job was sent back to re-review between the gate check and now.
            var rows = await _context.Applications
                .Where(a => a.ApplicationId == applicationId
                         && a.Job.RecruiterId == recruiterId
                         && (a.Status == null || a.Status.ToUpper() == "PENDING")
                         && (a.Job.Status == null || a.Job.Status.ToUpper() != "PENDING"))
                .ExecuteUpdateAsync(s => s.SetProperty(a => a.Status, status));

            if (rows == 0) return null; // not found, not owned, or already processed by someone else

            // Reload (include Job) to supply CandidateId / job title for the notification.
            return await _context.Applications
                .AsNoTracking()
                .Include(a => a.Job)
                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);
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

        // Temporar: this returns the set of distinct locations across the recruiter's
        // applicants and is deliberately NOT narrowed by the current tech/experience/keyword filters.
        // The location dropdown must stay stable so the user can switch locations without options
        // disappearing. Do NOT make it "dynamic" off the filtered query — that hurts UX and adds an
        // extra dependent query per filter change.
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
