using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using DevHub.Services.Interfaces;
using DevHub.Models;

namespace DevHub.Controllers
{
    [Authorize(AuthenticationSchemes = "Cookies,EmployerCookies,AdminCookies")]
    [Route("Notification")]
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("Recent")]
        public async Task<IActionResult> GetRecentNotifications([FromQuery] int limit = 10)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
                return Unauthorized();

            var userType = User.FindFirstValue(ClaimTypes.Role) ?? "";

            var notifications = await _notificationService.GetUserNotificationsAsync(userId, userType, limit);
            var unreadCount = await _notificationService.GetUnreadCountAsync(userId, userType);

            ViewBag.UnreadCount = unreadCount;
            ViewBag.UserType = userType;
            return PartialView("~/Views/Shared/_NotificationDropdown.cshtml", notifications);
        }

        [HttpPost("MarkAsRead/{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out _))
                return Unauthorized();

            await _notificationService.MarkAsReadAsync(id);
            return Ok(); // Không trả về JSON nữa
        }

        [HttpPost("MarkAllAsRead")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
                return Unauthorized();

            var userType = User.FindFirstValue(ClaimTypes.Role) ?? "";

            await _notificationService.MarkAllAsReadAsync(userId, userType);
            return Ok();
        }
    }
}
