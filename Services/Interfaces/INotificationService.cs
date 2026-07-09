using System.Collections.Generic;
using System.Threading.Tasks;
using DevHub.Validators.ViewModels.Shared;

namespace DevHub.Services.Interfaces;

public interface INotificationService
{
    Task SendNotificationAsync(int userId, string userType, string title, string message, string? type = null, string? severity = null, int? referenceId = null, string? referenceType = null);
    Task<List<NotificationItemViewModel>> GetUserNotificationsAsync(int userId, string userType, int limit = 20);
    Task<int> GetUnreadCountAsync(int userId, string userType);
    Task MarkAsReadAsync(int notificationId);
    Task MarkAllAsReadAsync(int userId, string userType);
}
