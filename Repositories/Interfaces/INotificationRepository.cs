using System.Collections.Generic;
using System.Threading.Tasks;
using DevHub.Models;

namespace DevHub.Repositories.Interfaces;

public interface INotificationRepository
{
    Task<Notification> CreateNotificationAsync(Notification notification);
    Task<List<Notification>> GetUserNotificationsAsync(int userId, string userType, int limit = 20);
    Task<int> GetUnreadCountAsync(int userId, string userType);
    Task MarkAsReadAsync(int notificationId);
    Task MarkAllAsReadAsync(int userId, string userType);
    Task<string?> GetFirstUserTypeByUserIdAsync(int userId);
}
