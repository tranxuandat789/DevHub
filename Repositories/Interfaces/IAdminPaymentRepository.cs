using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevHub.Models;

namespace DevHub.Repositories.Interfaces;

public interface IAdminPaymentRepository
{
    Task<(IEnumerable<PackageTransaction> Items, int TotalCount)> GetTransactionsAsync(
        DateTime? from, DateTime? to, int? serviceId, string? keyword, string? sortBy, int page, int pageSize);
        
    Task<PackageTransaction?> GetTransactionDetailAsync(int transactionId);
}
