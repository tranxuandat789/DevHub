using DevHub.Repositories.Interfaces;
using DevHub.Services.Interfaces;
using DevHub.ViewModels.Recruiter;

namespace DevHub.Services.Implementations
{
    public class RecruiterService : IRecruiterService
    {
        private readonly IRecruiterRepository _recruiterProfileRepository;

        public RecruiterService(IRecruiterRepository recruiterProfileRepository)
        {
            _recruiterProfileRepository = recruiterProfileRepository;
        }

        public async Task<RecruiterProfileViewModel> GetProfileAsync(int recruiterId)
        {
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
            bool hasLicense = !string.IsNullOrEmpty(recruiter.BusinessLicenseUrl);
            bool hasAdditionalDocs = !string.IsNullOrEmpty(recruiter.AdditionalDocumentsUrl);

            int companyFields = companyProfile.Count(field => !string.IsNullOrEmpty(field));

            //each field contribution to profile completeness: company profile field 8%, business license 15%, additional documents 5%
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

            // Business rule: ensure TaxCode is unique across recruiters
            if (!string.IsNullOrEmpty(updateVm.TaxCode) && updateVm.TaxCode != existingRecruiter.TaxCode)
            {
                bool isTaken = await _recruiterProfileRepository.CheckTaxCodeExistAsync(updateVm.TaxCode, recruiterId);
                if (isTaken)
                    throw new InvalidOperationException("Mã số thuế này đã được đăng ký bởi một doanh nghiệp khác trên hệ thống.");
            }

            existingRecruiter.FullName = updateVm.FullName;
            existingRecruiter.Position = updateVm.Position;
            existingRecruiter.Phone = updateVm.Phone;
            existingRecruiter.CompanyName = updateVm.CompanyName;
            existingRecruiter.CompanyAddress = updateVm.CompanyAddress;
            existingRecruiter.CompanyLogoUrl = updateVm.CompanyLogoUrl;
            existingRecruiter.CompanyDescription = updateVm.CompanyDescription;
            existingRecruiter.Website = updateVm.Website;
            existingRecruiter.Industry = updateVm.Industry;
            existingRecruiter.TaxCode = updateVm.TaxCode;
            existingRecruiter.BusinessLicenseUrl = updateVm.BusinessLicenseUrl;
            existingRecruiter.AdditionalDocumentsUrl = updateVm.AdditionalDocumentsUrl;

            // Recalculate profile completeness and persist
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

            await _recruiterProfileRepository.UpdateProfileAsync(existingRecruiter);

            // Optional audit log when profile is nearly complete — must not fail the profile save
            if (profileCompleteness >= 90)
            {
                try
                {
                    string details = $"Profile completeness {profileCompleteness}% - recruiter {existingRecruiter.RecruiterId} requests verification";
                    await _recruiterProfileRepository.CreateVerificationRequestAsync(existingRecruiter.RecruiterId, details);
                }
                catch
                {
                    // audit_log may be missing or unavailable; profile update already committed
                }
            }
        }

        public async Task SendVerificationRequestAsync(int recruiterId)
        {
            var recruiter = await _recruiterProfileRepository.GetProfileAsync(recruiterId);
            if (recruiter == null)
                throw new KeyNotFoundException("Recruiter not found");

            if (string.IsNullOrEmpty(recruiter.BusinessLicenseUrl))
                throw new InvalidOperationException("Vui lòng cung cấp Giấy phép kinh doanh hợp lệ để thực hiện xác thực");

            // create verification request for moderators; do not throw if repository fails
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
    }
}
