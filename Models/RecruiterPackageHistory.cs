using System;
using System.Collections.Generic;

namespace DevHub.Models;

public partial class RecruiterPackageHistory
{
    public int Id { get; set; }

    public int RecruiterId { get; set; }

    public int ServiceId { get; set; }

    public int CreditGranted { get; set; }

    public int CreditRemaining { get; set; }

    public int PostsRemaining { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool? IsActive { get; set; }

    public decimal PriceAtPurchase { get; set; }

    public virtual Recruiter Recruiter { get; set; } = null!;

    public virtual ServicePackage Service { get; set; } = null!;
}
