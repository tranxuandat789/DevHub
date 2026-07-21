//AnhPT-11/07/2026
using System;
using System.Linq;
using System.Threading.Tasks;
using DevHub.Repositories.Interfaces;
using DevHub.Services.Interfaces;
using DevHub.ViewModels.Admin;
using DevHub.ViewModels.Payment;

namespace DevHub.Services.Implementations;

public class AdminPaymentService : IAdminPaymentService
{
    private readonly IAdminPaymentRepository _adminPaymentRepo;
    private readonly IServicePackageRepository _serviceRepo;

    public AdminPaymentService(
        IAdminPaymentRepository adminPaymentRepo,
        IServicePackageRepository serviceRepo)
    {
        _adminPaymentRepo = adminPaymentRepo;
        _serviceRepo = serviceRepo;
    }

    public async Task<AdminTransactionListVm> GetTransactionsAsync(DateTime? from, DateTime? to, int? serviceId, string? keyword, string? sortBy, int page)
    {
        int pageSize = 5;
        var (items, total) = await _adminPaymentRepo.GetTransactionsAsync(from, to, serviceId, keyword, sortBy, page, pageSize);

        var packages = await _serviceRepo.GetActiveAsync();
        
        var list = items.Select(t => new AdminTransactionItemVm
        {
            TransactionId = t.TransactionId,
            TransactionDate = t.TransactionDate ?? DateTime.UtcNow,
            CompanyName = t.Company?.CompanyName ?? "Unknown",
            PackageName = t.Service?.PackageName ?? "Unknown",
            Status = t.Status,
            FinalAmount = t.FinalAmount,
            TotalAmount = t.TotalAmount,
            TransactionType = t.TransactionType
        }).ToList();

        var availablePackages = packages.Select(p => new PackageVm
        {
            ServiceId = p.ServiceId,
            PackageName = p.PackageName,
            Title = p.Title,
            Price = p.Price
        }).ToList();

        return new AdminTransactionListVm
        {
            Items = list,
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling(total / (double)pageSize),
            AvailablePackages = availablePackages
        };
    }

    public async Task<AdminTransactionDetailVm?> GetTransactionDetailAsync(int id)
    {
        var tx = await _adminPaymentRepo.GetTransactionDetailAsync(id);
        if (tx == null) return null;

        return new AdminTransactionDetailVm
        {
            TransactionId = tx.TransactionId,
            CompanyName = tx.Company?.CompanyName ?? "Unknown",
            PackageName = tx.Service?.PackageName ?? "Unknown",
            TransactionDate = tx.TransactionDate ?? DateTime.UtcNow,
            AmountVnd = tx.AmountVnd,
            DiscountAmount = tx.DiscountAmount,
            FinalAmount = tx.FinalAmount,
            TotalAmount = tx.TotalAmount,
            VatRate = tx.VatRate,
            VatAmount = tx.VatAmount,
            Status = tx.Status,
            PaymentMethod = tx.PaymentMethod,
            VnpayTxnRef = tx.VnpayTxnRef,
            VnpayTransactionNo = tx.VnpayTransactionNo,
            VnpayBankCode = tx.VnpayBankCode,
            PromoCode = tx.Promotion?.PromoCode
        };
    }
}
