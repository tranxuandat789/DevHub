using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DevHub.Data;
using DevHub.Models;
using DevHub.Repositories.Interfaces;

namespace DevHub.Repositories.Implementations;

public class NotificationRepository : INotificationRepository
{
    private readonly ItrecruitmentDbContext _context;

    public NotificationRepository(ItrecruitmentDbContext context)
    {
        _context = context;
    }

    public async Task<Notification> CreateNotificationAsync(Notification notification)
    {
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
        return notification;
    }

    public async Task<List<Notification>> GetUserNotificationsAsync(int userId, string userType, int limit = 20)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId && n.UserType == userType)
            .OrderByDescending(n => n.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(int userId, string userType)
    {
        return await _context.Notifications
            .CountAsync(n => n.UserId == userId && n.UserType == userType && (n.IsRead == false || n.IsRead == null));
    }

    public async Task MarkAsReadAsync(int notificationId)
    {
        var notif = await _context.Notifications.FindAsync(notificationId);
        if (notif != null && (notif.IsRead == false || notif.IsRead == null))
        {
            notif.IsRead = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task MarkAllAsReadAsync(int userId, string userType)
    {
        var unreadNotifs = await _context.Notifications
            .Where(n => n.UserId == userId && n.UserType == userType && (n.IsRead == false || n.IsRead == null))
            .ToListAsync();

        foreach (var n in unreadNotifs)
        {
            n.IsRead = true;
        }
        await _context.SaveChangesAsync();
    }

    public async Task<string?> GetFirstUserTypeByUserIdAsync(int userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId)
            .Select(n => n.UserType)
            .FirstOrDefaultAsync();
    }
}
