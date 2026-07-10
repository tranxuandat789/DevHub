using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using DevHub.Data;

namespace DevHub.Controllers.Recruiter
{
    [Route("recruiter/notifications")]
    [Authorize(Roles = "RECRUITER")]
    public class RecruiterNotificationController : Controller
    {
        private readonly ItrecruitmentDbContext _db;

        public RecruiterNotificationController(ItrecruitmentDbContext db)
        {
            _db = db;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(int page = 1, string status = "all", string sort = "newest")
        {
            ViewData["ActiveMenu"] = "Notifications";
            const int pageSize = 5;
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int userId))
            {
                var query = _db.Notifications
                    .Where(n => n.UserId == userId && n.UserType == "RECRUITER");

                if (status == "read")
                {
                    query = query.Where(n => n.IsRead == true);
                }
                else if (status == "unread")
                {
                    query = query.Where(n => n.IsRead != true);
                }

                if (sort == "oldest")
                {
                    query = query.OrderBy(n => n.CreatedAt);
                }
                else
                {
                    query = query.OrderByDescending(n => n.CreatedAt);
                }

                var totalItems = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

                var notifications = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var appIds = notifications.Where(n => n.ReferenceType == "Application" && n.ReferenceId.HasValue).Select(n => n.ReferenceId.Value).ToList();
                var jobIds = notifications.Where(n => n.ReferenceType == "JobPost" && n.ReferenceId.HasValue).Select(n => n.ReferenceId.Value).ToList();
                var interviewIds = notifications.Where(n => n.ReferenceType == "Interview" && n.ReferenceId.HasValue).Select(n => n.ReferenceId.Value).ToList();

                var appsData = await _db.Applications.Include(a => a.Candidate).Include(a => a.Job).ThenInclude(j => j.Company).Where(a => appIds.Contains(a.ApplicationId)).ToDictionaryAsync(a => a.ApplicationId);
                var jobsData = await _db.JobPosts.Include(j => j.Company).Where(j => jobIds.Contains(j.JobId)).ToDictionaryAsync(j => j.JobId);
                var interviewsData = await _db.Interviews.Include(i => i.Application).ThenInclude(a => a.Job).Where(i => interviewIds.Contains(i.InterviewId)).ToDictionaryAsync(i => i.InterviewId);

                ViewBag.ApplicationsData = appsData;
                ViewBag.JobsData = jobsData;
                ViewBag.InterviewsData = interviewsData;

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.TotalItems = totalItems;
                ViewBag.CurrentStatus = status;
                ViewBag.CurrentSort = sort;
                return View(notifications);
            }

            ViewBag.CurrentPage = 1;
            ViewBag.TotalPages = 1;
            ViewBag.TotalItems = 0;
            return View(new System.Collections.Generic.List<DevHub.Models.Notification>());
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            ViewData["ActiveMenu"] = "Notifications";
            ViewBag.NotificationId = id;
            var notification = await _db.Notifications.FindAsync(id);
            if (notification != null)
            {
                if (notification.IsRead != true)
                {
                    notification.IsRead = true;
                    await _db.SaveChangesAsync();
                }

                switch (notification.ReferenceType)
                {
                    case "Application":
                        if (notification.ReferenceId.HasValue)
                        {
                            ViewBag.ApplicationData = await _db.Applications
                                .Include(a => a.Job).ThenInclude(j => j.Company)
                                .Include(a => a.Candidate)
                                .FirstOrDefaultAsync(a => a.ApplicationId == notification.ReferenceId.Value);
                        }
                        break;
                    case "JobPost":
                        if (notification.ReferenceId.HasValue)
                        {
                            ViewBag.JobData = await _db.JobPosts
                                .Include(j => j.Company)
                                .FirstOrDefaultAsync(j => j.JobId == notification.ReferenceId.Value);
                        }
                        break;
                    case "Interview":
                        if (notification.ReferenceId.HasValue)
                        {
                            ViewBag.InterviewData = await _db.Interviews
                                .Include(i => i.Application).ThenInclude(a => a.Job)
                                .FirstOrDefaultAsync(i => i.InterviewId == notification.ReferenceId.Value);
                        }
                        break;
                    case "Company":
                        if (notification.ReferenceId.HasValue)
                        {
                            ViewBag.CompanyData = await _db.Companies
                                .FirstOrDefaultAsync(c => c.CompanyId == notification.ReferenceId.Value);
                        }
                        break;
                    case "Review":
                        if (notification.ReferenceId.HasValue)
                        {
                            ViewBag.ReviewData = await _db.ReviewCompanies
                                .Include(r => r.Company)
                                .FirstOrDefaultAsync(r => r.ReviewId == notification.ReferenceId.Value);
                        }
                        break;
                    case "Payment":
                        if (notification.ReferenceId.HasValue)
                        {
                            ViewBag.PaymentData = await _db.PackageTransactions
                                .Include(t => t.Service)
                                .FirstOrDefaultAsync(t => t.TransactionId == notification.ReferenceId.Value);
                        }
                        break;
                    case "Article":
                        if (notification.ReferenceId.HasValue)
                        {
                            ViewBag.ArticleData = await _db.Articles
                                .FirstOrDefaultAsync(a => a.ArticleId == notification.ReferenceId.Value);
                        }
                        break;
                }
            }
            return View(notification);
        }
    }
}
