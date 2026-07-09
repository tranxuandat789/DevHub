using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using DevHub.Hubs;
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using DevHub.Services.Interfaces;
using DevHub.Validators.ViewModels.Shared;

namespace DevHub.Services.Implementations;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(
        INotificationRepository notificationRepository, 
        IHubContext<NotificationHub> hubContext)
    {
        _notificationRepository = notificationRepository;
        _hubContext = hubContext;
    }

    public async Task SendNotificationAsync(int userId, string userType, string title, string message, string? type = null, string? severity = null, int? referenceId = null, string? referenceType = null)
    {
        var notif = new Notification
        {
            UserId = userId,
            UserType = userType,
            Title = title,
            Message = message,
            Type = type,
            SeverityLevel = severity,
            ReferenceId = referenceId,
            ReferenceType = referenceType,
            IsRead = false,
            CreatedAt = DateTime.Now
        };

        var savedNotif = await _notificationRepository.CreateNotificationAsync(notif);

        var vm = new NotificationItemViewModel
        {
            NotificationId = savedNotif.NotificationId,
            Title = savedNotif.Title,
            Message = savedNotif.Message,
            Type = savedNotif.Type,
            IsRead = savedNotif.IsRead ?? false,
            CreatedAt = savedNotif.CreatedAt,
            SeverityLevel = savedNotif.SeverityLevel,
            ReferenceId = savedNotif.ReferenceId,
            ReferenceType = savedNotif.ReferenceType
        };

        // Gửi qua SignalR cho user cụ thể (dựa trên userId dưới dạng chuỗi)
        await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", vm);
    }

    public async Task<List<NotificationItemViewModel>> GetUserNotificationsAsync(int userId, string userType, int limit = 20)
    {
        var notifs = await _notificationRepository.GetUserNotificationsAsync(userId, userType, limit);
        return notifs.Select(n => new NotificationItemViewModel
        {
            NotificationId = n.NotificationId,
            Title = n.Title,
            Message = n.Message,
            Type = n.Type,
            IsRead = n.IsRead ?? false,
            CreatedAt = n.CreatedAt,
            SeverityLevel = n.SeverityLevel,
            ReferenceId = n.ReferenceId,
            ReferenceType = n.ReferenceType
        }).ToList();
    }

    public async Task<int> GetUnreadCountAsync(int userId, string userType)
    {
        return await _notificationRepository.GetUnreadCountAsync(userId, userType);
    }

    public async Task MarkAsReadAsync(int notificationId)
    {
        await _notificationRepository.MarkAsReadAsync(notificationId);
    }

    public async Task MarkAllAsReadAsync(int userId, string userType)
    {
        await _notificationRepository.MarkAllAsReadAsync(userId, userType);
    }
}
