using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using DevHub.Data;
using DevHub.Services.Interfaces;
using DevHub.Validators.ViewModels.Shared;

namespace DevHub.Controllers
{
    [Authorize]
    [Route("Notification")]
    public class NotificationController : Controller
    {
        private readonly ItrecruitmentDbContext _db;
        private readonly INotificationService _notificationService;

        public NotificationController(ItrecruitmentDbContext db, INotificationService notificationService)
        {
            _db = db;
            _notificationService = notificationService;
        }

        [HttpGet("Recent")]
        public async Task<IActionResult> GetRecentNotifications([FromQuery] int limit = 10)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
                return Unauthorized();


            var notifications = await _db.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(limit)
                .ToListAsync();

            var unreadCount = notifications.Count(n => n.IsRead != true);
            var userType = (User.FindFirstValue(ClaimTypes.Role) ?? "").ToUpper();

            var viewModels = notifications.Select(n => new NotificationItemViewModel
            {
                NotificationId = n.NotificationId,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                IsRead = n.IsRead == true,
                CreatedAt = n.CreatedAt,
                SeverityLevel = n.SeverityLevel,
                ReferenceId = n.ReferenceId,
                ReferenceType = n.ReferenceType
            }).ToList();

            ViewBag.UnreadCount = unreadCount;
            ViewBag.UserType = string.IsNullOrEmpty(userType) ? notifications.FirstOrDefault()?.UserType ?? "CANDIDATE" : userType;
            return PartialView("~/Views/Shared/_NotificationDropdown.cshtml", viewModels);
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

            var userType = (User.FindFirstValue(ClaimTypes.Role) ?? "").ToUpper();
            await _notificationService.MarkAllAsReadAsync(userId, userType);
            return Ok();
        }
    }
}
