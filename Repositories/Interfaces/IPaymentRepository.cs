using DevHub.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevHub.Repositories.Interfaces;

public interface IPaymentRepository
{
    Task<List<Promotion>> GetActivePromotionsAsync();
    Task<Promotion?> GetValidPromotionByCodeAsync(string code);
    Task<int> CreateTransactionAsync(PackageTransaction tx);
    Task<PackageTransaction?> GetByTxnRefAsync(string vnpTxnRef);
    Task<PackageTransaction?> GetByIdForCompanyAsync(int txId, int companyId);
    
    Task<(List<PackageTransaction> Items, int Total)> GetHistoryAsync(
        int companyId, DateTime? from, DateTime? to, int? serviceId, int page, int pageSize);
        
    Task<List<PackageTransaction>> GetHistoryForExportAsync(
        int companyId, DateTime? from, DateTime? to, int? serviceId);

    Task<CompanyPackageHistory?> GetActivePackageAsync(int companyId);

    // Idempotent confirm
    Task<bool> MarkPaidAndActivateAsync(string vnpTxnRef, string vnpTransactionNo, string vnpBankCode);
    Task MarkFailedAsync(string vnpTxnRef, string vnpTransactionNo, string? bankCode);
}
