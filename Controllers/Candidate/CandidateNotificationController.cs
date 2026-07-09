using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using DevHub.Data;

namespace DevHub.Controllers.Candidate
{
    [Route("candidate/notifications")]
    [Authorize(Roles = "CANDIDATE")]
    public class CandidateNotificationController : Controller
    {
        private readonly ItrecruitmentDbContext _db;

        public CandidateNotificationController(ItrecruitmentDbContext db)
        {
            _db = db;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            ViewData["ActiveMenu"] = "Notifications";
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int userId))
            {
                var notifications = await _db.Notifications
                    .Where(n => n.UserId == userId && n.UserType == "CANDIDATE")
                    .OrderByDescending(n => n.CreatedAt)
                    .ToListAsync();
                return View(notifications);
            }
            
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
                notification.IsRead = true;
                await _db.SaveChangesAsync();
            }
            return View(notification);
        }
    }
}