using System;
using System.Collections.Generic;

namespace DevHub.Models;

public partial class AuditLog
{
    public int LogId { get; set; }

    public int? UserId { get; set; }

    public string? UserType { get; set; }

    public string Action { get; set; } = null!;

    public string? EntityType { get; set; }

    public int? EntityId { get; set; }

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public string? IpAddress { get; set; }

    public DateTime? CreatedAt { get; set; }
}
