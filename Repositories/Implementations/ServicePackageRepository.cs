using DevHub.Data;
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevHub.Repositories.Implementations;

public class ServicePackageRepository : IServicePackageRepository
{
    private readonly ItrecruitmentDbContext _context;
    
    public ServicePackageRepository(ItrecruitmentDbContext context)
    {
        _context = context;
    }

    public async Task<ServicePackage?> GetByIdAsync(int serviceId)
    {
        return await _context.ServicePackages.FirstOrDefaultAsync(s => s.ServiceId == serviceId);
    }

    public async Task<(List<ServicePackage> Items, int TotalCount)> GetAllPackagesAsync(string searchTerm, string statusFilter, string sortOrder, int page, int pageSize)
    {
        var query = _context.ServicePackages.AsQueryable();

        // Search
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(s => s.PackageName.ToLower().Contains(searchTerm) || s.Title.ToLower().Contains(searchTerm));
        }

        // Filter
        if (!string.IsNullOrWhiteSpace(statusFilter) && statusFilter.ToLower() != "all")
        {
            bool isActive = statusFilter.ToLower() == "active";
            query = query.Where(s => s.IsActive == isActive);
        }

        // Sort
        switch (sortOrder?.ToLower())
        {
            case "price_asc":
                query = query.OrderBy(s => s.Price);
                break;
            case "price_desc":
                query = query.OrderByDescending(s => s.Price);
                break;
            case "duration_desc":
                query = query.OrderByDescending(s => s.DurationDays ?? 0);
                break;
            default:
                query = query.OrderByDescending(s => s.CreatedAt);
                break;
        }

        int totalCount = await query.CountAsync();
        
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return (items, totalCount);
    }

    public async Task<ServicePackage> CreatePackageAsync(ServicePackage package)
    {
        package.CreatedAt = DateTime.UtcNow;
        if (package.IsActive == null) package.IsActive = true;
        
        _context.ServicePackages.Add(package);
        await _context.SaveChangesAsync();
        return package;
    }

    public async Task<ServicePackage> UpdatePackageAsync(ServicePackage package)
    {
        var existing = await _context.ServicePackages.FindAsync(package.ServiceId);
        if (existing != null)
        {
            existing.PackageName = package.PackageName;
            existing.Title = package.Title;
            existing.Price = package.Price;
            existing.Credit = package.Credit;
            existing.MaxPosts = package.MaxPosts;
            existing.DurationDays = package.DurationDays;
            existing.PriorityPush = package.PriorityPush;
            existing.HasAiChatbot = package.HasAiChatbot;
            existing.Description = package.Description;
            existing.ImageUrl = package.ImageUrl;
            
            await _context.SaveChangesAsync();
        }
        return existing ?? package;
    }

    public async Task<bool> ToggleStatusAsync(int id, bool activate)
    {
        var package = await _context.ServicePackages.FindAsync(id);
        if (package != null)
        {
            package.IsActive = activate;
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<List<ServicePackage>> GetActiveAsync()
    {
        return await _context.ServicePackages
            .Where(s => s.IsActive == true)
            .OrderBy(s => s.Price)
            .ToListAsync();
    }
}
