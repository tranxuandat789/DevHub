//AnhPT 03-06-2026
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using DevHub.Repositories.Interfaces;
using DevHub.Services.Interfaces;
using DevHub.ViewModels.Recruiter;

namespace DevHub.Services.Implementations
{
    public class RecruiterService : IRecruiterService
    {
        // Collapse consecutive whitespace into a single space and trim (preserves case).
        // e.g. "Lương    tháng 13" -> "Lương tháng 13"
        [return: NotNullIfNotNull(nameof(s))]
        private static string? NormalizeSpaces(string? s) =>
            string.IsNullOrWhiteSpace(s) ? s : Regex.Replace(s.Trim(), @"\s+", " ");

        //Repository interface instance
        private readonly IRecruiterRepository _recruiterProfileRepository;
        private readonly IUserAccountRepository _userRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IAssignModeratorService _assignModeratorService;
        private readonly INotificationService _notificationService;
        private readonly DevHub.Helpers.EmailHelper _emailHelper;

        //Constructor Injection
        public RecruiterService(
            IRecruiterRepository recruiterProfileRepository, 
            IUserAccountRepository userRepository, 
            ICompanyRepository companyRepository,
            IAssignModeratorService assignModeratorService,
            INotificationService notificationService,
            DevHub.Helpers.EmailHelper emailHelper)
        {
            _recruiterProfileRepository = recruiterProfileRepository;
            _userRepository = userRepository;
            _companyRepository = companyRepository;
            _assignModeratorService = assignModeratorService;
            _notificationService = notificationService;
            _emailHelper = emailHelper;
        }

        public async Task<RecruiterProfileViewModel> GetProfileAsync(int recruiterId)
        {
            //get recruiter profile by id
            var recruiter = await _recruiterProfileRepository.GetProfileAsync(recruiterId);
            if (recruiter == null)
                throw new Exception("Recruiter not found");

            var companyProfile = new List<string?>
            {
                recruiter.FullName,
                recruiter.Position,
                recruiter.Phone,
                recruiter.Company?.CompanyName,
                recruiter.Company?.CompanyAddress,
                recruiter.Company?.CompanyLogoUrl,
                recruiter.Company?.CompanyDescription,
                recruiter.Company?.Website,
                recruiter.Company?.Industry,
                recruiter.Company?.TaxCode
            };
            //Check if this recruiter has been upload licenses or not
            bool hasLicense = !string.IsNullOrEmpty(recruiter.Company?.BusinessLicenseUrl);
            bool hasAdditionalDocs = !string.IsNullOrEmpty(recruiter.Company?.AdditionalDocumentsUrl);

            int companyFields = companyProfile.Count(field => !string.IsNullOrEmpty(field));

            //each field contribution to profile completeness: company profile field 9%, business license 7%, additional documents 3%
            int profileCompleteness = companyFields * 9 + (hasLicense ? 7 : 0) + (hasAdditionalDocs ? 3 : 0);

            return new RecruiterProfileViewModel
            {
                FullName = recruiter.FullName,
                Position = recruiter.Position,
                Phone = recruiter.Phone,
                CompanyName = recruiter.Company?.CompanyName,
                CompanyAddress = recruiter.Company?.CompanyAddress,
                CompanyLogoUrl = recruiter.Company?.CompanyLogoUrl,
                CompanyDescription = recruiter.Company?.CompanyDescription,
                Website = recruiter.Company?.Website,
                Industry = recruiter.Company?.Industry,
                TaxCode = recruiter.Company?.TaxCode,
                BusinessLicenseUrl = recruiter.Company?.BusinessLicenseUrl,
                AdditionalDocumentsUrl = recruiter.Company?.AdditionalDocumentsUrl,
                IsVerified = recruiter.Company?.IsVerified,
                ProfileCompleteness = profileCompleteness
            };
        }

        public async Task UpdateProfileAsync(DevHub.Models.Recruiter existingRecruiter, RecruiterProfileViewModel updateVm)
        {
            if (existingRecruiter == null)
                throw new KeyNotFoundException("Recruiter not found");

            int recruiterId = existingRecruiter.RecruiterId;

            // Business rule: ensure TaxCode is unique.,
            if (!string.IsNullOrEmpty(updateVm.TaxCode) && updateVm.TaxCode != existingRecruiter.Company.TaxCode)
            {
                bool isTaken = await _recruiterProfileRepository.CheckTaxCodeExistAsync(updateVm.TaxCode, recruiterId);
                if (isTaken)
                    throw new InvalidOperationException("Mã số thuế này đã được đăng ký bởi một doanh nghiệp khác trên hệ thống.");
            }

            // Normalize blanks (collapse multiple spaces + trim) for free-text info fields on save.
            existingRecruiter.FullName = NormalizeSpaces(updateVm.FullName);
            existingRecruiter.Position = NormalizeSpaces(updateVm.Position);
            existingRecruiter.Phone = updateVm.Phone;
            existingRecruiter.Company.CompanyName = NormalizeSpaces(updateVm.CompanyName);
            existingRecruiter.Company.CompanyAddress = NormalizeSpaces(updateVm.CompanyAddress);
            existingRecruiter.Company.CompanyLogoUrl = updateVm.CompanyLogoUrl;
            existingRecruiter.Company.CompanyDescription = NormalizeSpaces(updateVm.CompanyDescription);
            existingRecruiter.Company.Website = updateVm.Website;
            existingRecruiter.Company.Industry = NormalizeSpaces(updateVm.Industry);
            existingRecruiter.Company.TaxCode = updateVm.TaxCode;
            existingRecruiter.Company.BusinessLicenseUrl = updateVm.BusinessLicenseUrl;
            existingRecruiter.Company.AdditionalDocumentsUrl = updateVm.AdditionalDocumentsUrl;

            // Recalculate profile completeness
            var companyProfile = new List<string?>
            {
                existingRecruiter.FullName,
                existingRecruiter.Position,
                existingRecruiter.Phone,
                existingRecruiter.Company.CompanyLogoUrl,
                existingRecruiter.Company.CompanyName,
                existingRecruiter.Company.CompanyAddress,
                existingRecruiter.Company.CompanyDescription,
                existingRecruiter.Company.Website,
                existingRecruiter.Company.Industry,
                existingRecruiter.Company.TaxCode
            };
            bool hasLicense = !string.IsNullOrEmpty(existingRecruiter.Company.BusinessLicenseUrl);
            bool hasAdditionalDocs = !string.IsNullOrEmpty(existingRecruiter.Company.AdditionalDocumentsUrl);
            int companyFields = companyProfile.Count(field => !string.IsNullOrEmpty(field));
            int profileCompleteness = companyFields * 9 + (hasLicense ? 7 : 0) + (hasAdditionalDocs ? 3 : 0);

            if(existingRecruiter.Company != null)
            {
                existingRecruiter.Company.ProfileCompletion = profileCompleteness;
            }

            //save changes to database  
            await _recruiterProfileRepository.UpdateProfileAsync(existingRecruiter);
        }

        public async Task RegisterCompanyProfileAsync(DevHub.Models.Recruiter existingRecruiter, RecruiterProfileViewModel updateVm)
        {
            if (existingRecruiter == null)
                throw new KeyNotFoundException("Recruiter not found");

            if (existingRecruiter.CompanyId != null)
                throw new InvalidOperationException("Recruiter already has a company");

            if (!string.IsNullOrEmpty(updateVm.TaxCode))
            {
                // Check if tax code exists globally
                bool isTaken = await _recruiterProfileRepository.CheckTaxCodeExistAsync(updateVm.TaxCode, existingRecruiter.RecruiterId);
                if (isTaken)
                    throw new InvalidOperationException("Mã số thuế này đã được đăng ký bởi một doanh nghiệp khác trên hệ thống.");
            }

            var companyProfile = new List<string?>
            {
                existingRecruiter.FullName,
                existingRecruiter.Position,
                existingRecruiter.Phone,
                updateVm.CompanyLogoUrl,
                updateVm.CompanyName,
                updateVm.CompanyAddress,
                updateVm.CompanyDescription,
                updateVm.Website,
                updateVm.Industry,
                updateVm.TaxCode
            };
            int companyFields = companyProfile.Count(field => !string.IsNullOrEmpty(field));
            int profileCompleteness = companyFields * 9; // no business license / additional docs at registration

            var newCompany = new DevHub.Models.Company
            {
                CompanyName = NormalizeSpaces(updateVm.CompanyName),
                TaxCode = updateVm.TaxCode,
                CompanyAddress = NormalizeSpaces(updateVm.CompanyAddress),
                CompanyDescription = NormalizeSpaces(updateVm.CompanyDescription),
                Website = updateVm.Website,
                Industry = NormalizeSpaces(updateVm.Industry),
                CompanyLogoUrl = updateVm.CompanyLogoUrl,
                IsVerified = false,
                ProfileCompletion = profileCompleteness
            };

            await _companyRepository.AddCompanyAsync(newCompany);

            existingRecruiter.CompanyId = newCompany.CompanyId;
            existingRecruiter.IsCompanyAdmin = true;
            await _recruiterProfileRepository.AssignCompanyAsync(existingRecruiter.RecruiterId, newCompany.CompanyId, true);
        }

        // User-triggered verification request. Only allowed when the company profile is more than 96% complete;
        public async Task SendVerificationRequestAsync(int recruiterId)
        {
            var recruiter = await _recruiterProfileRepository.GetProfileAsync(recruiterId);
            if (recruiter == null)
                throw new KeyNotFoundException("Recruiter not found");

            // Gate: profile completeness must exceed 96%.
            if ((recruiter.Company.ProfileCompletion ?? 0) <= 96)
                throw new InvalidOperationException("Hồ sơ công ty cần hoàn thiện trên 96% mới có thể gửi yêu cầu xác thực.");

            if (string.IsNullOrEmpty(recruiter.Company.BusinessLicenseUrl))
                throw new InvalidOperationException("Vui lòng cung cấp Giấy phép kinh doanh hợp lệ để thực hiện xác thực");

            if (recruiter.Company.IsVerified == true)
                throw new InvalidOperationException("Công ty của bạn đã được xác thực.");

            // Check if there is already a pending request
            if (await _recruiterProfileRepository.HasPendingVerificationRequestAsync(recruiterId))
            {
                throw new InvalidOperationException("Bạn đã gửi yêu cầu xác thực và đang chờ duyệt. Vui lòng không gửi lại.");
            }

            // create verification request for moderators, do not throw if repository fails to create log
            try
            {
                string details = $"Recruiter {recruiter.RecruiterId} requests verification. Company: {recruiter.Company.CompanyName}, TaxCode: {recruiter.Company.TaxCode}";
                await _recruiterProfileRepository.CreateVerificationRequestAsync(recruiter.RecruiterId, details);

                if (recruiter.CompanyId.HasValue)
                {
                    var assignedModId = await _assignModeratorService.AutoAssignNewRecordAsync("COMPANY_APPROVAL", recruiter.CompanyId.Value);
                    
                    if (assignedModId.HasValue)
                    {
                        await _notificationService.SendNotificationAsync(
                            userId: assignedModId.Value,
                            userType: "MODERATOR",
                            title: "Yêu cầu duyệt công ty mới",
                            message: $"Công ty '{recruiter.Company.CompanyName}' đang chờ bạn kiểm duyệt.",
                            type: "COMPANY_APPROVAL",
                            severity: "info",
                            referenceId: recruiter.CompanyId.Value,
                            referenceType: "Company"
                        );

                        var modAccount = await _userRepository.GetByIdAsync(assignedModId.Value);
                        if (modAccount != null && modAccount.EmailNotificationsEnabled && !string.IsNullOrEmpty(modAccount.Email))
                        {
                            string subject = "Bạn có công việc mới cần xử lý - DevHub";
                            string content = $@"
                                <p>Chào <strong>bạn</strong>,</p>
                                <p>Bạn vừa được gán <strong>1 Yêu cầu xác minh công ty</strong> mới cần xét duyệt trên hệ thống DevHub.</p>
                                <p>Vui lòng đăng nhập vào hệ thống quản trị để kiểm tra và xử lý kịp thời.</p>";
                            string body = DevHub.Helpers.EmailHelper.GetBaseTemplate("Công Việc Mới Trên DevHub", content);
                            await _emailHelper.SendEmailAsync(modAccount.Email, subject, body);
                        }
                    }
                }
            }
            catch
            {
                // swallow to avoid breaking user flow
            }
        }

        public async Task<bool> HasPendingVerificationRequestAsync(int recruiterId)
        {
            return await _recruiterProfileRepository.HasPendingVerificationRequestAsync(recruiterId);
        }

        // Change the recruiter's login password after verifying.
        public async Task ChangePasswordAsync(int recruiterId, RecruiterChangePasswordViewModel vm)
        {
            //get recruiter
            var recruiter = await _recruiterProfileRepository.GetProfileAsync(recruiterId);
            if (recruiter == null)
                throw new KeyNotFoundException("Recruiter not found");

            var currentHash = recruiter.RecruiterNavigation?.PasswordHash;

            // Google-login accounts (registered via Google, no local password) may SET a password
            // without entering an old one. Normal accounts must verify their current password.
            bool isGoogleOnly = string.IsNullOrEmpty(currentHash) || currentHash == "GOOGLE_OAUTH";
            if (!isGoogleOnly)
            {
                // Verify the supplied current password against the stored BCrypt hash.
                bool currentOk;
                try
                {
                    currentOk = BCrypt.Net.BCrypt.Verify(vm.CurrentPassword, currentHash);
                }
                catch
                {
                    currentOk = false;
                }
                if (!currentOk)
                    throw new InvalidOperationException("Mật khẩu hiện tại không đúng.");
            }

            // Hash and persist the new password (recruiterId == userId on user_account).
            var newHash = BCrypt.Net.BCrypt.HashPassword(vm.NewPassword);
            await _userRepository.UpdatePasswordAsync(recruiterId, newHash);
        }
    }
}


