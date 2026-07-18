using ClosedXML.Excel;
using DevHub.Helpers;
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using DevHub.Services.Interfaces;
using DevHub.ViewModels.Payment;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DevHub.Services.Implementations;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepo;
    private readonly IServicePackageRepository _serviceRepo;
    private readonly INotificationRepository _notificationRepo;
    private readonly IConfiguration _configuration;
    private readonly IRecruiterRepository _recruiterRepo;
    private readonly EmailHelper _emailHelper;

    public PaymentService(
        IPaymentRepository paymentRepo,
        IServicePackageRepository serviceRepo,
        INotificationRepository notificationRepo,
        IConfiguration configuration,
        IRecruiterRepository recruiterRepo,
        EmailHelper emailHelper)
    {
        _paymentRepo = paymentRepo;
        _serviceRepo = serviceRepo;
        _notificationRepo = notificationRepo;
        _configuration = configuration;
        _recruiterRepo = recruiterRepo;
        _emailHelper = emailHelper;
    }

    public async Task<SubscriptionPageVm> GetSubscriptionPageAsync(int companyId)
    {
        var activePackages = await _serviceRepo.GetActiveAsync();
        var activePromotions = await _paymentRepo.GetActivePromotionsAsync();
        var userActivePackage = await _paymentRepo.GetActivePackageAsync(companyId);

        var vm = new SubscriptionPageVm
        {
            Packages = activePackages.Select(p => new PackageVm
            {
                ServiceId = p.ServiceId,
                PackageName = p.PackageName,
                Title = p.Title,
                Price = p.Price,
                Credit = p.Credit,
                MaxPosts = p.MaxPosts,
                DurationDays = p.DurationDays ?? 0,
                PriorityPush = p.PriorityPush ?? 0,
                HasAiChatbot = p.HasAiChatbot ?? false,
                Description = p.Description,
                ImageUrl = p.ImageUrl
            }).ToList(),

            ActivePromotions = activePromotions.Select(p => new PromotionVm
            {
                PromotionId = p.PromotionId,
                PromoCode = p.PromoCode,
                DiscountPercent = p.DiscountPercent ?? 0,
                MaxDiscount = p.MaxDiscount,
                EndDate = p.EndDate.HasValue ? p.EndDate.Value.ToDateTime(TimeOnly.MinValue) : DateTime.MaxValue,
                Description = p.Title // Title used as Description
            }).ToList()
        };

        if (userActivePackage != null)
        {
            vm.ActivePackage = new RecruiterActivePackageVm
            {
                TransactionId = userActivePackage.TransactionId,
                ServiceId = userActivePackage.ServiceId,
                PackageName = userActivePackage.Service.PackageName,
                PriceAtPurchase = userActivePackage.PriceAtPurchase,
                PostsRemaining = userActivePackage.PostsRemaining,
                PostsGranted = userActivePackage.PostsGranted,
                EndDate = userActivePackage.EndDate ?? DateTime.UtcNow,
                Price = userActivePackage.Service.Price
            };
        }

        return vm;
    }

    public async Task<VoucherCheckResultVm> ValidateVoucherAsync(int companyId, int serviceId, string code)
    {
        var service = await _serviceRepo.GetByIdAsync(serviceId);
        if (service == null || service.IsActive != true)
            return new VoucherCheckResultVm { IsValid = false, Message = "Gói dịch vụ không hợp lệ." };

        decimal discountAmount = 0;
        decimal originalPrice = service.Price;

        if (!string.IsNullOrEmpty(code))
        {
            var promotion = await _paymentRepo.GetValidPromotionByCodeAsync(code);
            if (promotion == null)
            {
                return new VoucherCheckResultVm { IsValid = false, Message = "Mã giảm giá không hợp lệ, đã hết hạn hoặc hết số lượng." };
            }

            discountAmount = Math.Round(originalPrice * (promotion.DiscountPercent ?? 0) / 100);
            if (promotion.MaxDiscount.HasValue && discountAmount > promotion.MaxDiscount.Value)
            {
                discountAmount = promotion.MaxDiscount.Value;
            }
        }

        // Calculate Deduction for Upgrade
        decimal deductionAmount = 0;
        var activePackage = await _paymentRepo.GetActivePackageAsync(companyId);

        if (activePackage != null)
        {
            if (service.Price < activePackage.Service.Price)
            {
                return new VoucherCheckResultVm { IsValid = false, Message = "Không thể hạ bậc gói dịch vụ." };
            }
            else if (service.Price > activePackage.Service.Price)
            {
                // Upgrade prorating
                double postRatio = activePackage.PostsRemaining / (double)activePackage.PostsGranted;
                double timeRatio = 0;
                
                var totalDays = activePackage.Service.DurationDays ?? 0;
                var endDateValue = activePackage.EndDate ?? DateTime.UtcNow;
                var remainingDays = (endDateValue - DateTime.UtcNow).TotalDays;
                if (totalDays > 0 && remainingDays > 0)
                {
                    timeRatio = remainingDays / (double)totalDays;
                }

                // Protect against ratios > 1 just in case
                postRatio = Math.Min(1.0, Math.Max(0.0, postRatio));
                timeRatio = Math.Min(1.0, Math.Max(0.0, timeRatio));

                deductionAmount = Math.Round((decimal)(0.9 * postRatio + 0.1 * timeRatio) * activePackage.PriceAtPurchase);
            }
        }

        decimal finalAmount = originalPrice - discountAmount - deductionAmount;
        if (finalAmount < 0) finalAmount = 0;

        decimal vatRate = _configuration.GetValue<decimal>("Tax:VatRatePercent", 8m);
        decimal vatAmount = Math.Round(finalAmount * vatRate / 100m, 0, MidpointRounding.AwayFromZero);
        decimal totalAmount = finalAmount + vatAmount;

        return new VoucherCheckResultVm
        {
            IsValid = true,
            Message = "Áp dụng thành công.",
            DiscountAmount = discountAmount,
            DeductionAmount = deductionAmount,
            FinalAmount = finalAmount,
            VatRate = vatRate,
            VatAmount = vatAmount,
            TotalAmount = totalAmount
        };
    }

    public async Task<(bool Success, string? Url, string? Error, bool IsFreeUpgrade)> CreatePaymentUrlAsync(int companyId, CreatePaymentRequestVm req, string clientIp)
    {
        var service = await _serviceRepo.GetByIdAsync(req.ServiceId);
        if (service == null || service.IsActive != true)
            return (false, null, "Gói dịch vụ không hợp lệ.", false);

        var voucherCheck = await ValidateVoucherAsync(companyId, req.ServiceId, req.PromoCode ?? "");
        if (!voucherCheck.IsValid)
            return (false, null, voucherCheck.Message, false);

        int? promotionId = null;
        if (!string.IsNullOrEmpty(req.PromoCode))
        {
            var promotion = await _paymentRepo.GetValidPromotionByCodeAsync(req.PromoCode);
            if (promotion != null) promotionId = promotion.PromotionId;
        }

        string txnRef = DateTime.UtcNow.ToString("yyyyMMddHHmmss") + "_" + companyId;
        
        string? buyerTaxCode = await _paymentRepo.GetBuyerTaxCodeAsync(companyId);

        var tx = new PackageTransaction
        {
            CompanyId = companyId,
            ServiceId = req.ServiceId,
            AmountVnd = service.Price,
            DiscountAmount = voucherCheck.DiscountAmount,
            FinalAmount = voucherCheck.FinalAmount,
            VatRate = voucherCheck.VatRate,
            VatAmount = voucherCheck.VatAmount,
            TotalAmount = voucherCheck.TotalAmount,
            BuyerTaxCode = buyerTaxCode,
            PaymentMethod = "VNPAY",
            VnpayTxnRef = txnRef,
            Status = "PENDING",
            TransactionType = "PURCHASE", // Or UPGRADE/RENEW
            PromotionId = promotionId,
            TransactionDate = DateTime.UtcNow
        };

        var activePackage = await _paymentRepo.GetActivePackageAsync(companyId);
        if (activePackage != null)
        {
            tx.TransactionType = service.Price > activePackage.Service.Price ? "UPGRADE" : "RENEW";
        }

        await _paymentRepo.CreateTransactionAsync(tx);

        if (voucherCheck.FinalAmount <= 0)
        {
            // Free Upgrade / Fully Discounted
            var confirmResult = await _paymentRepo.MarkPaidAndActivateAsync(txnRef, "FREE", "FREE");
            if (confirmResult)
            {
                // Send notification to company (UserType=COMPANY) or we could just skip for now since company doesn't have a direct UserId.
                return (true, null, null, true);
            }
            return (false, null, "Lỗi khi kích hoạt gói.", false);
        }

        // Generate VNPay URL
        var vnpay = new VnpayLibrary();
        vnpay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"] ?? "2.1.0");
        vnpay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"] ?? "pay");
        vnpay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]!);
        
        // Amount must be multiplied by 100
        var amount = (long)(voucherCheck.TotalAmount * 100);
        vnpay.AddRequestData("vnp_Amount", amount.ToString());
        vnpay.AddRequestData("vnp_CreateDate", DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss")); // VNPay requires GMT+7
        vnpay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"] ?? "VND");
        vnpay.AddRequestData("vnp_IpAddr", clientIp);
        vnpay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"] ?? "vn");
        vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toan don hang {txnRef}");
        vnpay.AddRequestData("vnp_OrderType", "other");
        vnpay.AddRequestData("vnp_ReturnUrl", _configuration["Vnpay:ReturnUrl"]!);
        vnpay.AddRequestData("vnp_TxnRef", txnRef);
        vnpay.AddRequestData("vnp_ExpireDate", DateTime.UtcNow.AddHours(7).AddMinutes(15).ToString("yyyyMMddHHmmss"));

        string paymentUrl = vnpay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"]!, _configuration["Vnpay:HashSecret"]!);

        return (true, paymentUrl, null, false);
    }

    public async Task<(bool Success, string ResponseCode, string Message)> ConfirmAsync(IDictionary<string, string> vnpParams)
    {
        var vnpay = new VnpayLibrary();
        foreach (var kv in vnpParams)
        {
            if (!string.IsNullOrEmpty(kv.Key) && kv.Key.StartsWith("vnp_"))
            {
                vnpay.AddResponseData(kv.Key, kv.Value);
            }
        }

        var vnp_SecureHash = vnpParams.TryGetValue("vnp_SecureHash", out var sh) ? sh : "";
        bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _configuration["Vnpay:HashSecret"]!);

        if (!checkSignature)
        {
            return (false, "97", "Chữ ký không hợp lệ");
        }

        var txnRef = vnpay.GetResponseData("vnp_TxnRef");
        var rspCode = vnpay.GetResponseData("vnp_ResponseCode");
        var bankCode = vnpay.GetResponseData("vnp_BankCode");
        var vnpTransactionNo = vnpay.GetResponseData("vnp_TransactionNo");

        var tx = await _paymentRepo.GetByTxnRefAsync(txnRef);
        if (tx == null)
            return (false, "01", "Không tìm thấy giao dịch");

        if (tx.Status == "SUCCESS")
            return (true, "00", "Giao dịch đã được xác nhận thành công (Idempotent)");

        long vnpAmount = 0;
        long.TryParse(vnpay.GetResponseData("vnp_Amount"), out vnpAmount);
        
        long expectedAmount = tx.TotalAmount > 0 ? (long)(tx.TotalAmount * 100) : (long)(tx.FinalAmount * 100);
        if (vnpAmount != expectedAmount)
        {
            return (false, "04", "Số tiền không hợp lệ");
        }

        if (rspCode == "00")
        {
            bool activated = await _paymentRepo.MarkPaidAndActivateAsync(txnRef, vnpTransactionNo, bankCode);
            if (activated)
            {
                await SendTransactionEmailAsync(txnRef, true);
                return (true, "00", "Thành công");
            }
            return (false, "99", "Lỗi nội bộ khi kích hoạt");
        }
        else
        {
            await _paymentRepo.MarkFailedAsync(txnRef, vnpTransactionNo, bankCode);
            await SendTransactionEmailAsync(txnRef, false);
            return (false, rspCode, "Thanh toán bị hủy hoặc thất bại");
        }
    }

    public async Task<PaymentHistoryListVm> GetHistoryAsync(int companyId, DateTime? from, DateTime? to, int? serviceId, int page)
    {
        int pageSize = 5;
        var (items, total) = await _paymentRepo.GetHistoryAsync(companyId, from, to, serviceId, page, pageSize);

        var list = items.Select(t => new PaymentHistoryItemVm
        {
            TransactionId = t.TransactionId,
            TransactionDate = t.TransactionDate ?? DateTime.UtcNow,
            PackageName = t.Service?.PackageName ?? "Unknown",
            Status = t.Status,
            FinalAmount = t.FinalAmount,
            TotalAmount = t.TotalAmount,
            TransactionType = t.TransactionType
        }).ToList();

        return new PaymentHistoryListVm
        {
            Items = list,
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling(total / (double)pageSize)
        };
    }

    public async Task<PaymentHistoryDetailVm?> GetHistoryDetailAsync(int companyId, int transactionId)
    {
        var tx = await _paymentRepo.GetByIdForCompanyAsync(transactionId, companyId);
        if (tx == null) return null;

        return new PaymentHistoryDetailVm
        {
            TransactionId = tx.TransactionId,
            PackageName = tx.Service?.PackageName ?? "Unknown",
            TransactionDate = tx.TransactionDate ?? DateTime.UtcNow,
            AmountVnd = tx.AmountVnd,
            DiscountAmount = tx.DiscountAmount,
            FinalAmount = tx.FinalAmount,
            Status = tx.Status,
            PaymentMethod = tx.PaymentMethod,
            VnpayTxnRef = tx.VnpayTxnRef,
            VnpayTransactionNo = tx.VnpayTransactionNo,
            VnpayBankCode = tx.VnpayBankCode,
            PromoCode = tx.Promotion?.PromoCode,
            VatRate = tx.VatRate,
            VatAmount = tx.VatAmount,
            TotalAmount = tx.TotalAmount,
            BuyerTaxCode = tx.BuyerTaxCode
        };
    }

    public async Task<byte[]> ExportHistoryAsync(int companyId, DateTime? from, DateTime? to, int? serviceId)
    {
        var data = await _paymentRepo.GetHistoryForExportAsync(companyId, from, to, serviceId);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Lịch sử thanh toán");

        // Headers
        worksheet.Cell(1, 1).Value = "Mã Giao Dịch";
        worksheet.Cell(1, 2).Value = "Ngày Giao Dịch";
        worksheet.Cell(1, 3).Value = "Gói Dịch Vụ";
        worksheet.Cell(1, 4).Value = "Loại";
        worksheet.Cell(1, 5).Value = "Trạng Thái";
        worksheet.Cell(1, 6).Value = "Tổng Tiền (VND)";
        worksheet.Cell(1, 7).Value = "Mã Giảm Giá";

        // Data
        int row = 2;
        foreach (var tx in data)
        {
            worksheet.Cell(row, 1).Value = tx.VnpayTxnRef ?? tx.TransactionId.ToString();
            worksheet.Cell(row, 2).Value = tx.TransactionDate?.ToString("dd/MM/yyyy HH:mm:ss");
            worksheet.Cell(row, 3).Value = tx.Service.PackageName;
            worksheet.Cell(row, 4).Value = tx.TransactionType;
            worksheet.Cell(row, 5).Value = tx.Status;
            worksheet.Cell(row, 6).Value = tx.FinalAmount;
            worksheet.Cell(row, 7).Value = tx.Promotion?.PromoCode;
            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private async Task SendTransactionEmailAsync(string txnRef, bool success)
    {
        try
        {
            var tx = await _paymentRepo.GetByTxnRefAsync(txnRef);
            if (tx == null) return;

            var recruiters = await _recruiterRepo.GetRecruitersByCompanyIdAsync(tx.CompanyId);
            if (recruiters == null || !recruiters.Any()) return;

            var typeStr = (tx.TransactionType?.ToUpper()) switch {
                "RENEW" => "gia hạn",
                "UPGRADE" => "nâng cấp gói",
                "BUY_PACKAGE" => "mua mới",
                "PURCHASE" => "mua mới",
                _ => "mua"
            };
            
            var packageName = tx.Service?.PackageName ?? "Gói dịch vụ";
            
            var subject = success ? $"Xác nhận thanh toán thành công {packageName} - DevHub" : $"Giao dịch thất bại {packageName} - DevHub";
            
            var headerText = success ? $"Chúc mừng! Bạn đã hoàn thành giao dịch {typeStr} {packageName}" : $"Giao dịch {typeStr} {packageName} của bạn không thành công";
            var headerColor = success ? "#4640DE" : "#DC2626";
            var boxStyle = success ? "background: #ECFDF5; border: 1px dashed #16A34A; color: #16A34A;" : "background: #FEF2F2; border: 1px dashed #DC2626; color: #DC2626;";
            var boxText = success ? "✓ Thanh toán thành công" : "✗ Thanh toán thất bại";

            var discountRow = "";
            if (tx.DiscountAmount > 0 || tx.FinalAmount < tx.AmountVnd)
            {
                var diff = tx.AmountVnd - tx.FinalAmount;
                discountRow = $@"
                <div style='display: flex; justify-content: space-between; margin-bottom: 10px;'>
                    <span style='color: #515B6F;'>Khấu trừ/Chiết khấu:</span>
                    <span style='font-weight: bold; color: #DC2626;'>- {diff.ToString("N0")} VNĐ</span>
                </div>";
            }

            foreach (var r in recruiters)
            {
                var email = r.RecruiterNavigation?.Email;
                if (string.IsNullOrWhiteSpace(email)) continue;
                var name = string.IsNullOrWhiteSpace(r.FullName) ? "bạn" : r.FullName;
                
                var body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;
                            border: 1px solid #D6DDEB; border-radius: 8px;'>
                    <h2 style='color: {headerColor}; text-align: center;'>{headerText}</h2>
                    <p>Xin chào {name},</p>
                    <p>Cảm ơn bạn đã tin tưởng và sử dụng dịch vụ của DevHub. Chi tiết giao dịch của bạn như sau:</p>
                    
                    <div style='background-color: #F8F9FA; padding: 16px; border-radius: 8px; margin: 20px 0;'>
                        <div style='display: flex; justify-content: space-between; margin-bottom: 10px;'>
                            <span style='color: #515B6F;'>Giá gốc:</span>
                            <span style='font-weight: bold;'>{tx.AmountVnd.ToString("N0")} VNĐ</span>
                        </div>
                        {discountRow}
                        <div style='display: flex; justify-content: space-between; margin-bottom: 10px;'>
                            <span style='color: #515B6F;'>Thuế GTGT ({tx.VatRate.ToString("0.##")}%):</span>
                            <span style='font-weight: bold;'>{tx.VatAmount.ToString("N0")} VNĐ</span>
                        </div>
                        <hr style='border: none; border-top: 1px solid #E5E5E5; margin: 12px 0;' />
                        <div style='display: flex; justify-content: space-between;'>
                            <span style='font-weight: bold; color: #25324B; font-size: 16px;'>Tổng chi phí:</span>
                            <span style='font-weight: bold; color: {headerColor}; font-size: 18px;'>{(tx.TotalAmount > 0 ? tx.TotalAmount : tx.FinalAmount).ToString("N0")} VNĐ</span>
                        </div>
                    </div>

                    <div style='text-align: center; margin: 28px 0;'>
                        <span style='display: inline-block; font-size: 18px; font-weight: bold; padding: 14px 28px; border-radius: 8px; {boxStyle}'>{boxText}</span>
                    </div>
                    <hr style='border: none; border-top: 1px solid #E5E5E5; margin: 20px 0;' />
                    <p style='font-size: 12px; color: #888888; text-align: center;'>Hệ thống tuyển dụng DevHub</p>
                </div>";

                await _emailHelper.SendEmailAsync(email, subject, body);
            }
        }
        catch
        {
            // Optionally log
        }
    }
}
