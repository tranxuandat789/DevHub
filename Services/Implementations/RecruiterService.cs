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

        //Constructor Injection
        public RecruiterService(IRecruiterRepository recruiterProfileRepository, IUserAccountRepository userRepository)
        {
            _recruiterProfileRepository = recruiterProfileRepository;
            _userRepository = userRepository;
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
                recruiter.CompanyName,
                recruiter.CompanyAddress,
                recruiter.CompanyLogoUrl,
                recruiter.CompanyDescription,
                recruiter.Website,
                recruiter.Industry,
                recruiter.TaxCode
            };
            //Check if this recruiter has been upload licenses or not
            bool hasLicense = !string.IsNullOrEmpty(recruiter.BusinessLicenseUrl);
            bool hasAdditionalDocs = !string.IsNullOrEmpty(recruiter.AdditionalDocumentsUrl);

            int companyFields = companyProfile.Count(field => !string.IsNullOrEmpty(field));

            //each field contribution to profile completeness: company profile field 9%, business license 7%, additional documents 3%
            int profileCompleteness = companyFields * 9 + (hasLicense ? 7 : 0) + (hasAdditionalDocs ? 3 : 0);

            return new RecruiterProfileViewModel
            {
                FullName = recruiter.FullName,
                Position = recruiter.Position,
                Phone = recruiter.Phone,
                CompanyName = recruiter.CompanyName,
                CompanyAddress = recruiter.CompanyAddress,
                CompanyLogoUrl = recruiter.CompanyLogoUrl,
                CompanyDescription = recruiter.CompanyDescription,
                Website = recruiter.Website,
                Industry = recruiter.Industry,
                TaxCode = recruiter.TaxCode,
                BusinessLicenseUrl = recruiter.BusinessLicenseUrl,
                AdditionalDocumentsUrl = recruiter.AdditionalDocumentsUrl,
                IsVerified = recruiter.IsVerified,
                ProfileCompleteness = profileCompleteness
            };
        }

        public async Task UpdateProfileAsync(DevHub.Models.Recruiter existingRecruiter, RecruiterProfileViewModel updateVm)
        {
            if (existingRecruiter == null)
                throw new KeyNotFoundException("Recruiter not found");

            int recruiterId = existingRecruiter.RecruiterId;

            // Business rule: ensure TaxCode is unique.,
            if (!string.IsNullOrEmpty(updateVm.TaxCode) && updateVm.TaxCode != existingRecruiter.TaxCode)
            {
                bool isTaken = await _recruiterProfileRepository.CheckTaxCodeExistAsync(updateVm.TaxCode, recruiterId);
                if (isTaken)
                    throw new InvalidOperationException("Mã số thuế này đã được đăng ký bởi một doanh nghiệp khác trên hệ thống.");
            }

            // Normalize blanks (collapse multiple spaces + trim) for free-text info fields on save.
            existingRecruiter.FullName = NormalizeSpaces(updateVm.FullName);
            existingRecruiter.Position = NormalizeSpaces(updateVm.Position);
            existingRecruiter.Phone = updateVm.Phone;
            existingRecruiter.CompanyName = NormalizeSpaces(updateVm.CompanyName);
            existingRecruiter.CompanyAddress = NormalizeSpaces(updateVm.CompanyAddress);
            existingRecruiter.CompanyLogoUrl = updateVm.CompanyLogoUrl;
            existingRecruiter.CompanyDescription = NormalizeSpaces(updateVm.CompanyDescription);
            existingRecruiter.Website = updateVm.Website;
            existingRecruiter.Industry = NormalizeSpaces(updateVm.Industry);
            existingRecruiter.TaxCode = updateVm.TaxCode;
            existingRecruiter.BusinessLicenseUrl = updateVm.BusinessLicenseUrl;
            existingRecruiter.AdditionalDocumentsUrl = updateVm.AdditionalDocumentsUrl;

            // Recalculate profile completeness
            var companyProfile = new List<string?>
            {
                existingRecruiter.FullName,
                existingRecruiter.Position,
                existingRecruiter.Phone,
                existingRecruiter.CompanyName,
                existingRecruiter.CompanyAddress,
                existingRecruiter.CompanyLogoUrl,
                existingRecruiter.CompanyDescription,
                existingRecruiter.Website,
                existingRecruiter.Industry,
                existingRecruiter.TaxCode
            };
            bool hasLicense = !string.IsNullOrEmpty(existingRecruiter.BusinessLicenseUrl);
            bool hasAdditionalDocs = !string.IsNullOrEmpty(existingRecruiter.AdditionalDocumentsUrl);
            int companyFields = companyProfile.Count(field => !string.IsNullOrEmpty(field));
            int profileCompleteness = companyFields * 9 + (hasLicense ? 7 : 0) + (hasAdditionalDocs ? 3 : 0);

            existingRecruiter.ProfileCompletion = profileCompleteness;

            //save changes to database  
            await _recruiterProfileRepository.UpdateProfileAsync(existingRecruiter);
        }

        // User-triggered verification request. Only allowed when the company profile is more than 96% complete;
        public async Task SendVerificationRequestAsync(int recruiterId)
        {
            var recruiter = await _recruiterProfileRepository.GetProfileAsync(recruiterId);
            if (recruiter == null)
                throw new KeyNotFoundException("Recruiter not found");

            // Gate: profile completeness must exceed 96%.
            if ((recruiter.ProfileCompletion ?? 0) <= 96)
                throw new InvalidOperationException("Hồ sơ công ty cần hoàn thiện trên 96% mới có thể gửi yêu cầu xác thực.");

            if (string.IsNullOrEmpty(recruiter.BusinessLicenseUrl))
                throw new InvalidOperationException("Vui lòng cung cấp Giấy phép kinh doanh hợp lệ để thực hiện xác thực");

            if (recruiter.IsVerified == true)
                throw new InvalidOperationException("Công ty của bạn đã được xác thực.");

            // Check if there is already a pending request
            if (await _recruiterProfileRepository.HasPendingVerificationRequestAsync(recruiterId))
            {
                throw new InvalidOperationException("Bạn đã gửi yêu cầu xác thực và đang chờ duyệt. Vui lòng không gửi lại.");
            }

            // create verification request for moderators, do not throw if repository fails to create log
            try
            {
                string details = $"Recruiter {recruiter.RecruiterId} requests verification. Company: {recruiter.CompanyName}, TaxCode: {recruiter.TaxCode}";
                await _recruiterProfileRepository.CreateVerificationRequestAsync(recruiter.RecruiterId, details);
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
