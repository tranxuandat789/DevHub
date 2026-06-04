using System;
using System.Collections.Generic;

namespace DevHub.Models;

public partial class UserAccount
{
    public int UserId { get; set; }

    public string? GoogleId { get; set; }

    public string Email { get; set; } = null!;

    public string? PasswordHash { get; set; }
    public string? OtpVerification { get; set; }

    public DateTime? OtpExpiresAt { get; set; } 
    public string? ResetPassworvmken { get; set; }

    public DateTime? ResetPasswordExpiresAt { get; set; }

    public string UserType { get; set; } = null!;

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? LastLogin { get; set; }

    public DateTime? LastUpdated { get; set; }

    public virtual Admin? Admin { get; set; }

    public virtual Candidate? Candidate { get; set; }

    public virtual Recruiter? Recruiter { get; set; }
}
