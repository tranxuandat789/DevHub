//DatTX-10/07/2026
using System;
using System.Linq;
using System.Threading.Tasks;
using DevHub.Data;
using DevHub.Models;
using DevHub.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Services.Implementations;

public class ModAssignmentService : IModAssignmentService
{
    private readonly ItrecruitmentDbContext _context;

    public ModAssignmentService(ItrecruitmentDbContext context)
    {
        _context = context;
    }

    public async Task<int?> GetAssignedModeratorAsync(int serviceId)
    {
        // Find all mods assigned to this service tier
        var modIds = await _context.ModTierAssignments
            .Where(a => a.ServiceId == serviceId)
            .Select(a => a.ModeratorId)
            .ToListAsync();

        if (!modIds.Any())
            return null; // No mod assigned specifically to this tier.

        // To balance load, we could find the mod with the fewest pending JobPosts + Articles.
        // For simplicity, we just pick a random mod from the assigned pool.
        var random = new Random();
        int index = random.Next(modIds.Count);
        return modIds[index];
    }

    public async Task AssignModeratorToTierAsync(int moderatorId, int serviceId)
    {
        var existing = await _context.ModTierAssignments
            .FirstOrDefaultAsync(a => a.ModeratorId == moderatorId && a.ServiceId == serviceId);
        
        if (existing == null)
        {
            _context.ModTierAssignments.Add(new ModTierAssignment
            {
                ModeratorId = moderatorId,
                ServiceId = serviceId,
                CreatedAt = DateTime.Now
            });
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveModeratorFromTierAsync(int moderatorId, int serviceId)
    {
        var existing = await _context.ModTierAssignments
            .FirstOrDefaultAsync(a => a.ModeratorId == moderatorId && a.ServiceId == serviceId);

        if (existing != null)
        {
            _context.ModTierAssignments.Remove(existing);
            await _context.SaveChangesAsync();
        }
    }
}
