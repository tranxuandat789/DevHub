using System;
using System.Collections.Generic;

namespace DevHub.ViewModels.Payment;

public class PackageVm
{
    public int ServiceId { get; set; }
    public string PackageName { get; set; } = null!;
    public string Title { get; set; } = null!;
    public decimal Price { get; set; }
    public int Credit { get; set; }
    public int MaxPosts { get; set; }
    public int DurationDays { get; set; }
    public int? PriorityPush { get; set; }
    public bool HasAiChatbot { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
}

public class PromotionVm
{
    public int PromotionId { get; set; }
    public string PromoCode { get; set; } = null!;
    public decimal DiscountPercent { get; set; }
    public decimal? MaxDiscount { get; set; }
    public DateTime EndDate { get; set; }
    public string? Description { get; set; }
}

public class RecruiterActivePackageVm
{
    public int TransactionId { get; set; }
    public int ServiceId { get; set; }
    public string PackageName { get; set; } = null!;
    public decimal PriceAtPurchase { get; set; }
    public int PostsRemaining { get; set; }
    public int PostsGranted { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Price { get; set; } // Current price of the service tier
}

public class SubscriptionPageVm
{
    public List<PackageVm> Packages { get; set; } = new();
    public List<PromotionVm> ActivePromotions { get; set; } = new();
    public RecruiterActivePackageVm? ActivePackage { get; set; }
}

public class VoucherCheckResultVm
{
    public bool IsValid { get; set; }
    public string? Message { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal DeductionAmount { get; set; }
    public decimal FinalAmount { get; set; }
}

public class CreatePaymentRequestVm
{
    public int ServiceId { get; set; }
    public string? PromoCode { get; set; }
}

public class PaymentHistoryItemVm
{
    public int TransactionId { get; set; }
    public DateTime TransactionDate { get; set; }
    public string PackageName { get; set; } = null!;
    public string Status { get; set; } = null!;
    public decimal FinalAmount { get; set; }
    public string TransactionType { get; set; } = null!;
}

public class PaymentHistoryListVm
{
    public List<PaymentHistoryItemVm> Items { get; set; } = new();
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
}

public class PaymentHistoryDetailVm
{
    public int TransactionId { get; set; }
    public string PackageName { get; set; } = null!;
    public DateTime TransactionDate { get; set; }
    public decimal AmountVnd { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public string Status { get; set; } = null!;
    public string PaymentMethod { get; set; } = null!;
    public string? VnpayTxnRef { get; set; }
    public string? VnpayTransactionNo { get; set; }
    public string? VnpayBankCode { get; set; }
    public string? PromoCode { get; set; }
}
