using DevHub.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevHub.Services.Interfaces;

public interface IPromotionService
{
    Task<(List<Promotion> Items, int TotalCount)> GetAllVouchersAsync(int page, int pageSize, string sortBy, string? keyword = null);
    Task<Promotion?> GetByIdAsync(int id);
    Task<(bool Success, Promotion? Promotion, string? ErrorMessage)> CreateAsync(Promotion promotion);
    Task<(bool Success, Promotion? Promotion, string? ErrorMessage)> UpdateAsync(Promotion promotion);
    Task<bool> DeactivateAsync(int id);
    Task<bool> ActivateAsync(int id);
    string GeneratePromoCode();
}
