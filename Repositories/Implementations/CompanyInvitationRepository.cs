using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHub.Data;
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Repositories.Implementations
{
    public class CompanyInvitationRepository : ICompanyInvitationRepository
    {
        private readonly ItrecruitmentDbContext _context;

        public CompanyInvitationRepository(ItrecruitmentDbContext context)
        {
            _context = context;
        }

        public async Task<CompanyInvitation> AddAsync(CompanyInvitation invitation)
        {
            await _context.CompanyInvitations.AddAsync(invitation);
            await _context.SaveChangesAsync();
            return invitation;
        }

        public async Task<CompanyInvitation?> GetByIdAsync(int id)
        {
            return await _context.CompanyInvitations
                .FirstOrDefaultAsync(i => i.InvitationId == id);
        }

        public async Task<CompanyInvitation?> GetByTokenAsync(string token)
        {
            return await _context.CompanyInvitations
                .Include(i => i.Company)
                .FirstOrDefaultAsync(i => i.Token == token);
        }

        public async Task<IEnumerable<CompanyInvitation>> GetPendingByCompanyIdAsync(int companyId)
        {
            return await _context.CompanyInvitations
                .Where(i => i.CompanyId == companyId && i.Status == "PENDING")
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
        }

        public async Task UpdateAsync(CompanyInvitation invitation)
        {
            _context.CompanyInvitations.Update(invitation);
            await _context.SaveChangesAsync();
        }
    }
}
