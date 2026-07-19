using DevHub.Models;
using DevHub.Repositories.Interfaces;
using DevHub.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevHub.Services.Implementations;

public class PromotionService : IPromotionService
{
    private readonly IPromotionRepository _promotionRepo;

    public PromotionService(IPromotionRepository promotionRepo)
    {
        _promotionRepo = promotionRepo;
    }

    public Task<(List<Promotion> Items, int TotalCount)> GetAllVouchersAsync(int page, int pageSize, string sortBy, string? keyword = null)
    {
        return _promotionRepo.GetAllVouchersAsync(page, pageSize, sortBy, keyword);
    }

    public Task<Promotion?> GetByIdAsync(int id)
    {
        return _promotionRepo.GetByIdAsync(id);
    }

    public async Task<(bool Success, Promotion? Promotion, string? ErrorMessage)> CreateAsync(Promotion promotion)
    {
        var validationError = ValidatePromotion(promotion);
        if (!string.IsNullOrEmpty(validationError))
            return (false, null, validationError);

        // Check duplicate code
        var existing = await _promotionRepo.GetByCodeAsync(promotion.PromoCode);
        if (existing != null)
            return (false, null, "Mã giảm giá này đã tồn tại.");

        var created = await _promotionRepo.CreateAsync(promotion);
        return (true, created, null);
    }

    public async Task<(bool Success, Promotion? Promotion, string? ErrorMessage)> UpdateAsync(Promotion promotion)
    {
        var validationError = ValidatePromotion(promotion);
        if (!string.IsNullOrEmpty(validationError))
            return (false, null, validationError);

        var existing = await _promotionRepo.GetByCodeAsync(promotion.PromoCode);
        if (existing != null && existing.PromotionId != promotion.PromotionId)
            return (false, null, "Mã giảm giá này đã tồn tại trên một chiến dịch khác.");

        var updated = await _promotionRepo.UpdateAsync(promotion);
        return (true, updated, null);
    }

    public Task<bool> DeactivateAsync(int id)
    {
        return _promotionRepo.DeactivateAsync(id);
    }

    public Task<bool> ActivateAsync(int id)
    {
        return _promotionRepo.ActivateAsync(id);
    }

    public string GeneratePromoCode()
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        int length = random.Next(8, 11); // 8-10 chars
        var stringChars = new char[length];

        for (int i = 0; i < length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }

        return new string(stringChars);
    }

    private string? ValidatePromotion(Promotion p)
    {
        if (string.IsNullOrWhiteSpace(p.Title))
            return "Tên voucher không được để trống.";
            
        if (string.IsNullOrWhiteSpace(p.PromoCode))
            return "Mã giảm giá không được để trống.";

        if (p.PromoCode.Length < 8 || p.PromoCode.Length > 10)
            return "Mã giảm giá phải từ 8 đến 10 ký tự.";

        if (p.DiscountPercent.HasValue && (p.DiscountPercent.Value < 0 || p.DiscountPercent.Value > 100))
            return "Phần trăm giảm giá phải nằm trong khoảng từ 0 đến 100.";

        if (p.MaxDiscount.HasValue && p.MaxDiscount.Value > 100000000)
            return "Số tiền giảm tối đa không được vượt quá 100,000,000 VNĐ.";

        if (p.StartDate.HasValue && p.EndDate.HasValue)
        {
            var start = p.StartDate.Value.ToDateTime(TimeOnly.MinValue);
            var end = p.EndDate.Value.ToDateTime(TimeOnly.MinValue);
            
            if (end < start.AddDays(1))
            {
                return "Ngày kết thúc phải sau ngày bắt đầu ít nhất 1 ngày.";
            }
        }

        return null;
    }
}
