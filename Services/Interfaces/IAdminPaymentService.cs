using System;
using System.Threading.Tasks;
using DevHub.ViewModels.Admin;

namespace DevHub.Services.Interfaces;

public interface IAdminPaymentService
{
    Task<AdminTransactionListVm> GetTransactionsAsync(DateTime? from, DateTime? to, int? serviceId, string? keyword, string? sortBy, int page);
    Task<AdminTransactionDetailVm?> GetTransactionDetailAsync(int id);
}
