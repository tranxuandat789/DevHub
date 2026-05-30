using System;
using System.Collections.Generic;

namespace DevHub.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int UserId { get; set; }

    public string UserType { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string? Type { get; set; }

    public bool? IsRead { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? SeverityLevel { get; set; }

    public int? ReferenceId { get; set; }

    public string? ReferenceType { get; set; }
}
