using System;
using System.Collections.Generic;

namespace DevHub.Models;

public partial class PackageTransaction
{
    public int TransactionId { get; set; }

    public int RecruiterId { get; set; }

    public int? ServiceId { get; set; }

    public decimal AmountVnd { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal FinalAmount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string? VnpayTxnRef { get; set; }

    public string? VnpayTransactionNo { get; set; }

    public string? VnpayBankCode { get; set; }

    public string Status { get; set; } = null!;

    public string TransactionType { get; set; } = null!;

    public int? PromotionId { get; set; }

    public string? Description { get; set; }

    public DateTime? TransactionDate { get; set; }

    public DateTime? CompletedAt { get; set; }

    public virtual Promotion? Promotion { get; set; }

    public virtual Recruiter Recruiter { get; set; } = null!;

    public virtual ServicePackage? Service { get; set; }

    public virtual ICollection<RecruiterPackageHistory> RecruiterPackageHistories { get; set; } = new List<RecruiterPackageHistory>();
}
