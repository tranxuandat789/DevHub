using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHub.Data;
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Repositories.Implementations;

public class AdminPaymentRepository : IAdminPaymentRepository
{
    private readonly ItrecruitmentDbContext _context;

    public AdminPaymentRepository(ItrecruitmentDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<PackageTransaction> Items, int TotalCount)> GetTransactionsAsync(
        DateTime? from, DateTime? to, int? serviceId, string? keyword, string? sortBy, int page, int pageSize)
    {
        var query = _context.PackageTransactions
            .Include(t => t.Company)
            .Include(t => t.Service)
            .AsQueryable();

        if (from.HasValue)
        {
            query = query.Where(t => t.TransactionDate >= from.Value.Date);
        }

        if (to.HasValue)
        {
            // Set to end of the day
            var toDate = to.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(t => t.TransactionDate <= toDate);
        }

        if (serviceId.HasValue && serviceId.Value > 0)
        {
            query = query.Where(t => t.ServiceId == serviceId.Value);
        }

        if (!string.IsNullOrEmpty(keyword))
        {
            var kw = keyword.Trim().ToLower();
            query = query.Where(t => t.Company.CompanyName.ToLower().Contains(kw));
        }

        // Sorting
        query = sortBy switch
        {
            "amount_asc" => query.OrderBy(t => t.FinalAmount),
            "amount_desc" => query.OrderByDescending(t => t.FinalAmount),
            "date_asc" => query.OrderBy(t => t.TransactionDate),
            _ => query.OrderByDescending(t => t.TransactionDate), // Default is date_desc
        };

        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return (items, total);
    }

    public async Task<PackageTransaction?> GetTransactionDetailAsync(int transactionId)
    {
        return await _context.PackageTransactions
            .Include(t => t.Company)
            .Include(t => t.Service)
            .Include(t => t.Promotion)
            .FirstOrDefaultAsync(t => t.TransactionId == transactionId);
    }
}
