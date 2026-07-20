using DevHub.Data;
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevHub.Repositories.Implementations;

public class PromotionRepository : IPromotionRepository
{
    private readonly ItrecruitmentDbContext _context;

    public PromotionRepository(ItrecruitmentDbContext context)
    {
        _context = context;
    }

    public async Task<(List<Promotion> Items, int TotalCount)> GetAllVouchersAsync(int page, int pageSize, string sortBy, string? keyword = null)
    {
        var query = _context.Promotions.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(p => p.Title.Contains(keyword) || p.PromoCode.Contains(keyword));
        }

        // sort bằng created date/ quantity
        if (sortBy == "quantity_asc")
            query = query.OrderBy(p => p.Quantity);
        else if (sortBy == "quantity_desc")
            query = query.OrderByDescending(p => p.Quantity);
        else if (sortBy == "date_asc")
            query = query.OrderBy(p => p.CreatedAt);
        else // default date_desc
            query = query.OrderByDescending(p => p.CreatedAt);

        int totalCount = await query.CountAsync();
        
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return (items, totalCount);
    }

    public async Task<Promotion?> GetByIdAsync(int id)
    {
        return await _context.Promotions.FirstOrDefaultAsync(p => p.PromotionId == id);
    }

    public async Task<Promotion?> GetByCodeAsync(string code)
    {
        return await _context.Promotions.FirstOrDefaultAsync(p => p.PromoCode.ToLower() == code.ToLower());
    }

    public async Task<Promotion> CreateAsync(Promotion promotion)
    {
        promotion.CreatedAt = DateTime.UtcNow;
        if (promotion.IsActive == null)
            promotion.IsActive = true;
            
        _context.Promotions.Add(promotion);
        await _context.SaveChangesAsync();
        return promotion;
    }

    public async Task<Promotion> UpdateAsync(Promotion promotion)
    {
        var existing = await _context.Promotions.FindAsync(promotion.PromotionId);
        if (existing != null)
        {
            existing.Title = promotion.Title;
            existing.PromoCode = promotion.PromoCode;
            existing.DiscountPercent = promotion.DiscountPercent;
            existing.MaxDiscount = promotion.MaxDiscount;
            existing.StartDate = promotion.StartDate;
            existing.EndDate = promotion.EndDate;
            existing.Quantity = promotion.Quantity;
            existing.IsActive = promotion.IsActive;

            await _context.SaveChangesAsync();
        }
        return existing ?? promotion;
    }

    public async Task<bool> DeactivateAsync(int id)
    {
        var existing = await _context.Promotions.FindAsync(id);
        if (existing != null)
        {
            existing.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<bool> ActivateAsync(int id)
    {
        var promotion = await _context.Promotions.FindAsync(id);
        if (promotion == null) return false;

        promotion.IsActive = true;
        await _context.SaveChangesAsync();
        return true;
    }
}
