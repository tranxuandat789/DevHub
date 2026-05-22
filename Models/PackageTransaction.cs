using System;
using System.Collections.Generic;

namespace DevHub.Models;

public partial class PackageTransaction
{
    public int TransactionId { get; set; }

    public int RecruiterId { get; set; }

    public int? ServiceId { get; set; }

    public int? JobId { get; set; }

    public int CreditUsed { get; set; }

    public int RemainingCredit { get; set; }

    public string TransactionType { get; set; } = null!;

    public int? PromotionId { get; set; }

    public string? Description { get; set; }

    public DateTime? TransactionDate { get; set; }

    public virtual JobPost? Job { get; set; }

    public virtual Promotion? Promotion { get; set; }

    public virtual Recruiter Recruiter { get; set; } = null!;

    public virtual ServicePackage? Service { get; set; }
}
