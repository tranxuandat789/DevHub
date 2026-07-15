using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHub.Helpers;
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using DevHub.Services.Interfaces;

namespace DevHub.Services.Implementations
{
    public class CompanyInvitationService : ICompanyInvitationService
    {
        private readonly ICompanyInvitationRepository _invitationRepo;
        private readonly IRecruiterRepository _recruiterRepo;
        private readonly ICompanyRepository _companyRepo;
        private readonly EmailHelper _emailHelper;

        public CompanyInvitationService(
            ICompanyInvitationRepository invitationRepo,
            IRecruiterRepository recruiterRepo,
            ICompanyRepository companyRepo,
            EmailHelper emailHelper)
        {
            _invitationRepo = invitationRepo;
            _recruiterRepo = recruiterRepo;
            _companyRepo = companyRepo;
            _emailHelper = emailHelper;
        }

        public async Task<bool> AcceptInvitationAsync(string token, int newRecruiterId)
        {
            var invitation = await _invitationRepo.GetByTokenAsync(token);
            if (invitation == null || invitation.Status != "PENDING" || invitation.ExpiresAt < DateTime.UtcNow)
            {
                return false;
            }

            var recruiter = await _recruiterRepo.GetProfileAsync(newRecruiterId);
            if (recruiter == null) return false;

            // Update recruiter company
            recruiter.CompanyId = invitation.CompanyId;
            recruiter.IsCompanyAdmin = false; // Invited members are not admins by default
            await _recruiterRepo.UpdateProfileAsync(recruiter);

            // Update invitation status
            invitation.Status = "ACCEPTED";
            await _invitationRepo.UpdateAsync(invitation);

            return true;
        }

        public async Task<bool> CancelInvitationAsync(int invitationId, int companyId)
        {
            var invitation = await _invitationRepo.GetByIdAsync(invitationId);
            if (invitation == null || invitation.CompanyId != companyId || invitation.Status != "PENDING")
            {
                return false;
            }

            invitation.Status = "EXPIRED";
            await _invitationRepo.UpdateAsync(invitation);
            return true;
        }

        public async Task<IEnumerable<CompanyInvitation>> GetPendingInvitationsAsync(int companyId)
        {
            return await _invitationRepo.GetPendingByCompanyIdAsync(companyId);
        }

        public async Task<CompanyInvitation> InviteMemberAsync(int companyId, string email, int invitedByRecruiterId)
        {
            var company = await _companyRepo.GetCompanyDetailsAsync(companyId);
            if (company == null) throw new Exception("Company not found");

            // Check if user is already a recruiter in the system 
            var pendingInvs = await _invitationRepo.GetPendingByCompanyIdAsync(companyId);
            if (pendingInvs.Any(i => i.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
            {
                throw new Exception("Email này đã được gửi lời mời và đang chờ xác nhận.");
            }

            var token = Guid.NewGuid().ToString("N");
            var invitation = new CompanyInvitation
            {
                CompanyId = companyId,
                Email = email.ToLower().Trim(),
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddDays(7), // 7 days expiration
                Status = "PENDING",
                CreatedAt = DateTime.UtcNow
            };

            await _invitationRepo.AddAsync(invitation);

            // Send Email
            var acceptLink = $"http://localhost:5111/Auth/AcceptInvite?token={token}"; // Hardcoded port for local, you can change to dynamic if needed
            var subject = $"Lời mời tham gia công ty {company.CompanyName} trên DevHub";
            var content = $@"
                <div style='text-align: center; margin-bottom: 30px;'>
                    <h2 style='color: #2D3748; margin-bottom: 15px;'>Lời Mời Tham Gia Công Ty</h2>
                    <p style='color: #4A5568; font-size: 16px;'>
                        Bạn đã được mời tham gia vào không gian làm việc của <strong>{company.CompanyName}</strong> trên hệ thống tuyển dụng DevHub.
                    </p>
                </div>
                <div style='text-align: center; margin-bottom: 30px;'>
                    <a href='{acceptLink}' style='display: inline-block; background-color: #4640DE; color: white; padding: 12px 30px; text-decoration: none; border-radius: 8px; font-weight: bold; font-size: 16px;'>
                        Chấp nhận lời mời
                    </a>
                </div>
                <p style='color: #FF3B30;'>Link này có hiệu lực trong vòng 7 ngày.</p>
                <p>Nếu bạn không muốn tham gia, vui lòng bỏ qua email này.</p>";

            var body = EmailHelper.GetBaseTemplate("Lời mời tham gia DevHub", content);
            await _emailHelper.SendEmailAsync(email, subject, body);

            return invitation;
        }

        public async Task<CompanyInvitation?> ValidateTokenAsync(string token)
        {
            var invitation = await _invitationRepo.GetByTokenAsync(token);
            if (invitation == null) return null;

            if (invitation.Status != "PENDING" || invitation.ExpiresAt < DateTime.UtcNow)
            {
                // Optionally update status to EXPIRED if it's past date
                if (invitation.Status == "PENDING" && invitation.ExpiresAt < DateTime.UtcNow)
                {
                    invitation.Status = "EXPIRED";
                    await _invitationRepo.UpdateAsync(invitation);
                }
                return null; // Invalid token
            }

            return invitation;
        }
    }
}
