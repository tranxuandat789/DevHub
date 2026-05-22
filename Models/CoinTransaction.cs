using System;
using System.Collections.Generic;

namespace DevHub.Models;

public partial class CoinTransaction
{
    public int TransactionId { get; set; }

    public int RecruiterId { get; set; }

    public decimal AmountVnd { get; set; }

    public decimal? CoinRate { get; set; }

    public int CoinAmount { get; set; }

    public int? BonusCoin { get; set; }

    public int TotalCoinReceived { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string? VnpayCode { get; set; }

    public string? Status { get; set; }

    public int? PromotionId { get; set; }

    public string? Description { get; set; }

    public DateTime? TransactionDate { get; set; }

    public int? ProcessedBy { get; set; }

    public virtual Admin? ProcessedByNavigation { get; set; }

    public virtual Promotion? Promotion { get; set; }

    public virtual Recruiter Recruiter { get; set; } = null!;
}
