using System;
using System.Collections.Generic;
using DevHub.ViewModels.Payment;

namespace DevHub.ViewModels.Admin;

public class AdminTransactionItemVm
{
    public int TransactionId { get; set; }
    public DateTime TransactionDate { get; set; }
    public string CompanyName { get; set; } = null!;
    public string PackageName { get; set; } = null!;
    public string Status { get; set; } = null!;
    public decimal FinalAmount { get; set; }
    public string TransactionType { get; set; } = null!;
}

public class AdminTransactionListVm
{
    public List<AdminTransactionItemVm> Items { get; set; } = new();
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public List<PackageVm> AvailablePackages { get; set; } = new();
}

public class AdminTransactionDetailVm
{
    public int TransactionId { get; set; }
    public string CompanyName { get; set; } = null!;
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
