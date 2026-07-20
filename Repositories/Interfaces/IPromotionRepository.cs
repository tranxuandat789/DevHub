using DevHub.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevHub.Repositories.Interfaces;

public interface IPromotionRepository
{
    Task<(List<Promotion> Items, int TotalCount)> GetAllVouchersAsync(int page, int pageSize, string sortBy, string? keyword = null);
    Task<Promotion?> GetByIdAsync(int id);
    Task<Promotion?> GetByCodeAsync(string code);
    Task<Promotion> CreateAsync(Promotion promotion);
    Task<Promotion> UpdateAsync(Promotion promotion);
    Task<bool> DeactivateAsync(int id);
    Task<bool> ActivateAsync(int id);
}
