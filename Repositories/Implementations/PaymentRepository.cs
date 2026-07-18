using DevHub.Data;
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevHub.Repositories.Implementations;

public class PaymentRepository : IPaymentRepository
{
    private readonly ItrecruitmentDbContext _context;

    public PaymentRepository(ItrecruitmentDbContext context)
    {
        _context = context;
    }

    public async Task<List<Promotion>> GetActivePromotionsAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return await _context.Promotions
            .Where(p => p.IsActive == true && p.StartDate <= today && p.EndDate >= today && p.Quantity > 0)
            .ToListAsync();
    }

    public async Task<Promotion?> GetValidPromotionByCodeAsync(string code)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return await _context.Promotions
            .FirstOrDefaultAsync(p => p.PromoCode == code && p.IsActive == true && 
                                      p.StartDate <= today && p.EndDate >= today && 
                                      p.Quantity > 0);
    }

    public async Task<int> CreateTransactionAsync(PackageTransaction tx)
    {
        _context.PackageTransactions.Add(tx);
        await _context.SaveChangesAsync();
        return tx.TransactionId;
    }

    public async Task<PackageTransaction?> GetByTxnRefAsync(string vnpTxnRef)
    {
        var tx = await _context.PackageTransactions
            .Include(t => t.Service)
            .Include(t => t.Promotion)
            .FirstOrDefaultAsync(t => t.VnpayTxnRef == vnpTxnRef);
        await CheckExpiredAsync(tx);
        return tx;
    }

    public async Task<string?> GetBuyerTaxCodeAsync(int companyId)
    {
        var company = await _context.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CompanyId == companyId);
        return company?.TaxCode;
    }

    public async Task<PackageTransaction?> GetByIdForCompanyAsync(int txId, int companyId)
    {
        var tx = await _context.PackageTransactions
            .Include(t => t.Service)
            .Include(t => t.Promotion)
            .FirstOrDefaultAsync(t => t.TransactionId == txId && t.CompanyId == companyId);
        await CheckExpiredAsync(tx);
        return tx;
    }

    public async Task<(List<PackageTransaction> Items, int Total)> GetHistoryAsync(
        int companyId, DateTime? from, DateTime? to, int? serviceId, int page, int pageSize)
    {
        var query = _context.PackageTransactions
            .Include(t => t.Service)
            .Where(t => t.CompanyId == companyId);

        if (from.HasValue)
        {
            query = query.Where(t => t.TransactionDate >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(t => t.TransactionDate <= to.Value.AddDays(1).AddTicks(-1));
        }

        if (serviceId.HasValue)
        {
            query = query.Where(t => t.ServiceId == serviceId.Value);
        }

        int total = await query.CountAsync();
        var items = await query.OrderByDescending(t => t.TransactionDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        await CheckExpiredListAsync(items);

        return (items, total);
    }

    public async Task<List<PackageTransaction>> GetHistoryForExportAsync(
        int companyId, DateTime? from, DateTime? to, int? serviceId)
    {
        var query = _context.PackageTransactions
            .Include(t => t.Service)
            .Include(t => t.Promotion)
            .Where(t => t.CompanyId == companyId);

        if (from.HasValue)
        {
            query = query.Where(t => t.TransactionDate >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(t => t.TransactionDate <= to.Value.AddDays(1).AddTicks(-1));
        }

        if (serviceId.HasValue)
        {
            query = query.Where(t => t.ServiceId == serviceId.Value);
        }

        var items = await query.OrderByDescending(t => t.TransactionDate).ToListAsync();
        await CheckExpiredListAsync(items);
        return items;
    }

    public async Task<CompanyPackageHistory?> GetActivePackageAsync(int companyId)
    {
        return await _context.CompanyPackageHistories
            .Include(h => h.Service)
            .Where(h => h.CompanyId == companyId && h.IsActive == true)
            .OrderByDescending(h => h.StartDate)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> MarkPaidAndActivateAsync(string vnpTxnRef, string vnpTransactionNo, string vnpBankCode)
    {
        var strategy = _context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var tx = await _context.PackageTransactions
                    .Include(t => t.Service)
                    .Include(t => t.Promotion)
                    .FirstOrDefaultAsync(t => t.VnpayTxnRef == vnpTxnRef);

                if (tx == null || tx.Status != "PENDING")
                {
                    return false;
                }

                tx.Status = "SUCCESS";
                tx.VnpayTransactionNo = vnpTransactionNo;
                tx.VnpayBankCode = vnpBankCode;
                tx.CompletedAt = DateTime.UtcNow;

                var activePackage = await GetActivePackageAsync(tx.CompanyId);
                var newHistory = new CompanyPackageHistory
                {
                    CompanyId = tx.CompanyId,
                    ServiceId = tx.ServiceId ?? 0,
                    TransactionId = tx.TransactionId,
                    IsActive = true,
                    PriceAtPurchase = tx.FinalAmount,
                    StartDate = DateTime.UtcNow,
                };

                if (activePackage != null)
                {
                    activePackage.IsActive = false;
                    _context.CompanyPackageHistories.Update(activePackage);

                    // Stacking (Same Tier)
                    if (tx.Service!.Price == activePackage.Service.Price)
                    {
                        newHistory.PostsGranted = tx.Service.MaxPosts;
                        newHistory.PostsRemaining = tx.Service.MaxPosts + activePackage.PostsRemaining;
                        newHistory.PromotionsRemaining = (tx.Service.PriorityPush ?? 0) + (activePackage.PromotionsRemaining);
                        
                        // Extend duration
                        var daysToAdd = tx.Service.DurationDays ?? 0;
                        // If current end date is in the future, extend it. Otherwise, start from now.
                        var baseDate = (activePackage.EndDate != null && activePackage.EndDate.Value > DateTime.UtcNow) ? activePackage.EndDate.Value : DateTime.UtcNow;
                        newHistory.EndDate = baseDate.AddDays(daysToAdd);
                    }
                    // Upgrade (Higher Tier)
                    else if (tx.Service.Price > activePackage.Service.Price)
                    {
                        newHistory.PostsGranted = tx.Service.MaxPosts;
                        newHistory.PostsRemaining = tx.Service.MaxPosts;
                        newHistory.PromotionsRemaining = tx.Service.PriorityPush ?? 0;
                        newHistory.EndDate = DateTime.UtcNow.AddDays(tx.Service.DurationDays ?? 0);
                    }
                }
                else
                {
                    // No active package
                    newHistory.PostsGranted = tx.Service.MaxPosts;
                    newHistory.PostsRemaining = tx.Service.MaxPosts;
                    newHistory.PromotionsRemaining = tx.Service.PriorityPush ?? 0;
                    newHistory.EndDate = DateTime.UtcNow.AddDays(tx.Service.DurationDays ?? 0);
                }

                _context.CompanyPackageHistories.Add(newHistory);

                if (tx.PromotionId.HasValue && tx.Promotion != null)
                {
                    if (tx.Promotion.Quantity > 0)
                    {
                        tx.Promotion.Quantity -= 1;
                        _context.Promotions.Update(tx.Promotion);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }

    public async Task MarkFailedAsync(string vnpTxnRef, string vnpTransactionNo, string? bankCode)
    {
        var tx = await _context.PackageTransactions.FirstOrDefaultAsync(t => t.VnpayTxnRef == vnpTxnRef);
        if (tx != null && tx.Status == "PENDING")
        {
            tx.Status = "FAILED";
            tx.VnpayTransactionNo = vnpTransactionNo;
            tx.VnpayBankCode = bankCode;
            tx.CompletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    private async Task CheckExpiredAsync(PackageTransaction? tx)
    {
        if (tx != null && tx.Status == "PENDING" && tx.TransactionDate.HasValue && tx.TransactionDate.Value.AddMinutes(15) <= DateTime.UtcNow)
        {
            tx.Status = "CANCELLED";
            tx.CompletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    private async Task CheckExpiredListAsync(IEnumerable<PackageTransaction> txs)
    {
        bool changed = false;
        foreach (var tx in txs)
        {
            if (tx.Status == "PENDING" && tx.TransactionDate.HasValue && tx.TransactionDate.Value.AddMinutes(15) <= DateTime.UtcNow)
            {
                tx.Status = "CANCELLED";
                tx.CompletedAt = DateTime.UtcNow;
                changed = true;
            }
        }
        if (changed)
        {
            await _context.SaveChangesAsync();
        }
    }
}
