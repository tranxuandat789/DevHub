using DevHub.ViewModels.Payment;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevHub.Services.Interfaces;

public interface IPaymentService
{
    Task<SubscriptionPageVm> GetSubscriptionPageAsync(int companyId);
    Task<VoucherCheckResultVm> ValidateVoucherAsync(int companyId, int serviceId, string code);
    
    // Returns (IsSuccess, PaymentUrl, ErrorMessage, IsFreeUpgrade)
    Task<(bool Success, string? Url, string? Error, bool IsFreeUpgrade)> CreatePaymentUrlAsync(int companyId, int recruiterId, CreatePaymentRequestVm req, string clientIp);
    
    // Confirm VNPay request
    Task<(bool Success, string ResponseCode, string Message)> ConfirmAsync(IDictionary<string, string> vnpParams);
    
    Task<PaymentHistoryListVm> GetHistoryAsync(int companyId, DateTime? from, DateTime? to, int? serviceId, int page);
    Task<PaymentHistoryDetailVm?> GetHistoryDetailAsync(int companyId, int transactionId);
    
    Task<byte[]> ExportHistoryAsync(int companyId, DateTime? from, DateTime? to, int? serviceId);
}
