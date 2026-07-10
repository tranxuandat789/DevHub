using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using DevHub.Services.Interfaces;
using DevHub.Validators.ViewModels.Shared;
using DevHub.Data;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Controllers.Moderator
{
    [Authorize(Roles = "Moderator")]
    [Route("ModeratorNotification")]
    public class ModeratorNotificationController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly ItrecruitmentDbContext _db;

        public ModeratorNotificationController(INotificationService notificationService, ItrecruitmentDbContext db)
        {
            _notificationService = notificationService;
            _db = db;
        }

        [HttpGet("Recent")]
        public async Task<IActionResult> GetRecentNotifications([FromQuery] int limit = 10)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
                return Unauthorized();

            var userType = "MODERATOR";
            var notifications = await _notificationService.GetUserNotificationsAsync(userId, userType, limit);
            var unreadCount = await _notificationService.GetUnreadCountAsync(userId, userType);

            ViewBag.UnreadCount = unreadCount;
            ViewBag.UserType = userType;
            return PartialView("~/Views/Shared/_NotificationDropdown.cshtml", notifications);
        }
        
        [HttpGet("UnreadCount")]
        public async Task<IActionResult> GetUnreadCount()
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
                return Unauthorized();

            var userType = "MODERATOR";
            var unreadCount = await _notificationService.GetUnreadCountAsync(userId, userType);

            return Json(new { count = unreadCount });
        }

        [HttpPost("MarkAsRead/{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out _))
                return Unauthorized();

            await _notificationService.MarkAsReadAsync(id);
            return Ok();
        }

        [HttpPost("MarkAllAsRead")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
                return Unauthorized();

            var userType = "MODERATOR";
            await _notificationService.MarkAllAsReadAsync(userId, userType);
            return Ok();
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(int page = 1)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
                return RedirectToAction("AdminLogin", "Auth");

            int pageSize = 15;
            var query = _db.Notifications
                .Where(n => n.UserId == userId && n.UserType == "MODERATOR")
                .OrderByDescending(n => n.CreatedAt);

            var totalItems = await query.CountAsync();
            var totalPages = (int)System.Math.Ceiling((double)totalItems / pageSize);
            
            page = System.Math.Max(1, System.Math.Min(page, totalPages > 0 ? totalPages : 1));

            var notifications = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(notifications);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
                return RedirectToAction("AdminLogin", "Auth");

            var notification = await _db.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == id && n.UserId == userId && n.UserType == "MODERATOR");

            if (notification == null)
            {
                return NotFound();
            }

            if (notification.IsRead != true)
            {
                await _notificationService.MarkAsReadAsync(id);
                notification.IsRead = true; // Update local state for view
            }

            return View(notification);
        }
    }
}
