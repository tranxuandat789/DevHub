using System;

namespace DevHub.Validators.ViewModels.Shared
{
    public class NotificationItemViewModel
    {
        public int NotificationId { get; set; }
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string? Type { get; set; }
        public bool IsRead { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? SeverityLevel { get; set; }
        public int? ReferenceId { get; set; }
        public string? ReferenceType { get; set; }
    }
}
