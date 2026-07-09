using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHub.Data;
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Repositories.Implementations
{
    public class IndustryAssignmentRepository : IIndustryAssignmentRepository
    {
        private readonly ItrecruitmentDbContext _context;

        public IndustryAssignmentRepository(ItrecruitmentDbContext context)
        {
            _context = context;
        }

        public async Task<int?> GetModeratorIdAsync(string taskType, string industry)
        {
            var assignment = await _context.ModeratorIndustryAssignments
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.TaskType == taskType && a.Industry == industry);
            return assignment?.ModeratorId;
        }

        public async Task<List<string>> GetIndustriesAsync(int moderatorId)
        {
            return await _context.ModeratorIndustryAssignments
                .AsNoTracking()
                .Where(a => a.ModeratorId == moderatorId)
                .Select(a => a.Industry)
                .ToListAsync();
        }

        public async Task SetIndustriesAsync(int moderatorId, string taskType, IEnumerable<string> industries, int assignedBy)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Delete existing ones for this moderator
                var existing = await _context.ModeratorIndustryAssignments
                    .Where(a => a.ModeratorId == moderatorId)
                    .ToListAsync();
                
                _context.ModeratorIndustryAssignments.RemoveRange(existing);
                await _context.SaveChangesAsync();

                // If industries is not null, add new ones
                if (industries != null && industries.Any())
                {
                    // Clean up from other moderators for these same industries in this task_type
                    var otherModsHoldingIndustries = await _context.ModeratorIndustryAssignments
                        .Where(a => a.TaskType == taskType && industries.Contains(a.Industry))
                        .ToListAsync();
                        
                    _context.ModeratorIndustryAssignments.RemoveRange(otherModsHoldingIndustries);
                    await _context.SaveChangesAsync();

                    var newAssignments = industries.Select(ind => new ModeratorIndustryAssignment
                    {
                        ModeratorId = moderatorId,
                        TaskType = taskType,
                        Industry = ind,
                        AssignedBy = assignedBy,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });

                    await _context.ModeratorIndustryAssignments.AddRangeAsync(newAssignments);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<int?> GetOwnerOfIndustryAsync(string taskType, string industry, int excludeModeratorId)
        {
            var owner = await _context.ModeratorIndustryAssignments
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.TaskType == taskType && a.Industry == industry && a.ModeratorId != excludeModeratorId);
            return owner?.ModeratorId;
        }

        public async Task<List<ModeratorIndustryAssignment>> GetAllByTaskTypeAsync(string taskType)
        {
            return await _context.ModeratorIndustryAssignments
                .AsNoTracking()
                .Include(a => a.Moderator)
                .Where(a => a.TaskType == taskType)
                .ToListAsync();
        }
    }
}
