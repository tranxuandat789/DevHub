//03/06/2026 DatTX
using DevHub.Services.Interfaces;
using DevHub.Repositories.Interfaces;
using DevHub.Models;


namespace DevHub.Services.Implementations
{
    public class CandidateService : ICandidateService
    {
        private readonly ICandidateRepository _candidateRepository;
        // Đây là Constructor để "tiêm" ICandidateRepository vào Service sử dụng
        public CandidateService(ICandidateRepository candidateRepository)
        {
            _candidateRepository = candidateRepository;
        }
        // get candidate by id with details (skills, cvs, etc) for profile page
        public async Task<DevHub.Models.Candidate?> GetCandidateByIdAsync(int candidateId)
        {
            return await _candidateRepository.GetByIdWithDetailsAsync(candidateId);
        }
        // update candidate profile and then recalculate the completion percentage
        public async Task UpdateProfileAsync(int candidateId, string fullName, string? phone, DateOnly? birthdate, string? gender, string? address, string? socialMediaUrl, decimal? expectedSalaryMin, decimal? expectedSalaryMax, string? preferredLocation, int? experienceYears, bool cvSearchable)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Họ và tên không được để trống!");
            if (fullName.Length > 100)
                throw new ArgumentException("Họ và tên không được vượt quá 100 ký tự!");

            // 2. Validate Số điện thoại (Bắt đầu bằng 0, đủ 10 số)
            if (!string.IsNullOrWhiteSpace(phone))
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(phone, "^0[0-9]{9}$"))
                    throw new ArgumentException("Số điện thoại không hợp lệ. Phải bắt đầu bằng số 0 và đúng 10 số!");
            }

            if (birthdate.HasValue)
            {
                var today = DateOnly.FromDateTime(DateTime.Now);
                var bDate = birthdate.Value;

                if (bDate > today)
                    throw new ArgumentException("Ngày sinh không thể nằm ở tương lai!");

                int age = today.Year - bDate.Year;
                if (bDate.AddYears(age) > today) age--;

                if (age < 15)
                    throw new ArgumentException($"Bạn mới {age} tuổi. Theo quy định, phải đủ 15 tuổi mới được tham gia tìm việc!");
            }

            // 4. Validate Link mạng xã hội
            if (!string.IsNullOrWhiteSpace(socialMediaUrl))
            {
                if (!socialMediaUrl.StartsWith("http://") && !socialMediaUrl.StartsWith("https://"))
                    throw new ArgumentException("Link mạng xã hội phải bắt đầu bằng http:// hoặc https://");
            }

            await _candidateRepository.UpdateProfileAsync(candidateId, fullName, phone, birthdate, gender, address, socialMediaUrl, expectedSalaryMin, expectedSalaryMax, preferredLocation, experienceYears, cvSearchable);
            await CalculateAndSaveCompletionAsync(candidateId);
        }

        // this method calculates the profile completion percentage based on the candidate's profile details and saves it to the database
        public async Task<int> CalculateAndSaveCompletionAsync(int candidateId)
        {
            var candidate = await _candidateRepository.GetByIdWithDetailsAsync(candidateId);
            if (candidate == null) return 0;
            int percent = 0;
            var cv = candidate.Cvs.FirstOrDefault(c => c.IsDefault == true);
            // setting percent
            // 1.Avatar
            if (!string.IsNullOrEmpty(candidate.ImageUrl)) percent += 10;
            // 2. CandidateInfo (20%)
            if (!string.IsNullOrEmpty(candidate.Phone)) percent += 3;
            if (candidate.Birthdate.HasValue) percent += 3;
            if (!string.IsNullOrEmpty(candidate.Gender)) percent += 3;
            if (!string.IsNullOrEmpty(candidate.Address)) percent += 3;
            if (!string.IsNullOrEmpty(candidate.PreferredLocation)) percent += 3;
            if (candidate.ExpectedSalaryMin.HasValue || candidate.ExpectedSalaryMax.HasValue) percent += 3;
            if (candidate.ExperienceYears.HasValue) percent += 2;
            // 3. Skills 
            if ((candidate.CandidateSkills != null && candidate.CandidateSkills.Any()) || (cv != null && !string.IsNullOrEmpty(cv.Skills) && cv.Skills.Length > 5)) percent += 15;
            // CV
            if (cv != null)
            {
                // 4. File PDF CV (40%)
                if (!string.IsNullOrEmpty(cv.CvUrl)) percent += 40;
                // 5. Education (5%) 
                if (!string.IsNullOrEmpty(cv.Education) && cv.Education.Length > 5) percent += 5;
                // 6. Experience (5%)
                if (!string.IsNullOrEmpty(cv.Experience) && cv.Experience.Length > 5) percent += 5;
                // 7. Languages (5%)
                if (!string.IsNullOrEmpty(cv.Languages) && cv.Languages.Length > 5) percent += 5;
            }
            if (percent > 100) percent = 100;
            await _candidateRepository.UpdateProfileCompletionAsync(candidateId, percent);
            return percent;
        }
    }
}
