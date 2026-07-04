using System;

namespace DevHub.Models;

public partial class CompanyInvitation
{
    public int InvitationId { get; set; }

    public int CompanyId { get; set; }

    public string Email { get; set; } = null!;

    public string Token { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public string Status { get; set; } = "PENDING";

    public DateTime? CreatedAt { get; set; }

    public virtual Company Company { get; set; } = null!;
}
