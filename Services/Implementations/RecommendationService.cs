//03/06/2026 DatTX
//Refactored v4 — Comprehensive scoring with full data normalization
using DevHub.Data;
using DevHub.Services.Interfaces;
using DevHub.ViewModels.Jobs;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Services.Implementations
{
    public class RecommendationService : IRecommendationService
    {
        private readonly ItrecruitmentDbContext _db;

        public RecommendationService(ItrecruitmentDbContext db)
        {
            _db = db;
        }

        public async Task<List<JobRecommendationViewModel>> GetRecommendedJobsAsync(int candidateId)
        {
            var candidate = await _db.Candidates
                .Include(c => c.CandidateSkills)
                .FirstOrDefaultAsync(c => c.CandidateId == candidateId);

            if (candidate == null)
                return new List<JobRecommendationViewModel>();

            var activeJobs = await _db.JobPosts
                .Include(j => j.Teches)
                .Include(j => j.Recruiter)
                .Where(j => j.Status == "APPROVED" && j.Deadline >= DateOnly.FromDateTime(DateTime.Now))
                .OrderByDescending(j => j.CreatedAt)
                .Take(200)
                .ToListAsync();

            // Chuẩn bị dữ liệu ứng viên 1 lần duy nhất (ngoài vòng lặp)
            var candidateSkills = candidate.CandidateSkills?.ToList() ?? new List<DevHub.Models.CandidateSkill>();
            var candidateTechIds = candidateSkills.Select(cs => cs.TechId).ToHashSet();

            var recommendations = new List<JobRecommendationViewModel>();

            foreach (var job in activeJobs)
            {
                double score = CalculateTotalScore(candidate, job, candidateSkills, candidateTechIds);
                if (score >= 50)
                {
                    recommendations.Add(new JobRecommendationViewModel
                    {
                        Job = job,
                        MatchPercentage = score
                    });
                }
            }

            return recommendations.OrderByDescending(r => r.MatchPercentage).Take(50).ToList();
        }

        // ============================================================
        // TỔNG ĐIỂM = 5 Module
        // Max = 35 + 20 + 20 + 15 + 10 = 100
        // ============================================================
        private double CalculateTotalScore(
            DevHub.Models.Candidate candidate,
            DevHub.Models.JobPost job,
            List<DevHub.Models.CandidateSkill> candidateSkills,
            HashSet<int> candidateTechIds)
        {
            double total = 0;
            total += CalculateSkillScore(candidateSkills, candidateTechIds, job);       // Max 35
            total += CalculateExperienceScore(candidate, job);                          // Max 20
            total += CalculateWorkingModelScore(candidate, job);                        // Max 20
            total += CalculateSalaryScore(candidate, job);                              // Max 15
            total += CalculateLocationScore(candidate, job);                            // Max 10
            return Math.Min(100, Math.Round(total, 1));
        }

        // ============================================================
        // MODULE 1: KỸ NĂNG + TRÌNH ĐỘ (Max 35đ)
        // 1A: Coverage (20đ) — tỷ lệ tech trùng khớp
        // 1B: Level (15đ) — trình độ ứng viên vs yêu cầu job
        // ============================================================
        private double CalculateSkillScore(
            List<DevHub.Models.CandidateSkill> candidateSkills,
            HashSet<int> candidateTechIds,
            DevHub.Models.JobPost job)
        {
            if (job.Teches == null || !job.Teches.Any())
                return 22; // Job không gắn tech → điểm sàn

            var jobTechIds = job.Teches.Select(t => t.TechId).ToList();
            var matchedTechIds = jobTechIds.Where(id => candidateTechIds.Contains(id)).ToList();
            int matchedCount = matchedTechIds.Count;

            // 1A: Skill Coverage (Max 20đ)
            double coverageScore = ((double)matchedCount / jobTechIds.Count) * 20;

            // 1B: Skill Level Matching (Max 15đ)
            double levelScore = 0;
            if (matchedCount > 0)
            {
                int jobLevelRank = GetLevelRank(job.ExperienceLevel);
                double totalFactor = 0;

                foreach (var techId in matchedTechIds)
                {
                    var cs = candidateSkills.FirstOrDefault(s => s.TechId == techId);
                    int candidateRank = GetLevelRank(cs?.Level);
                    int diff = candidateRank - jobLevelRank;

                    if (diff >= 0)
                        totalFactor += 1.0;   // Bằng hoặc cao hơn yêu cầu
                    else if (diff == -1)
                        totalFactor += 0.5;   // Thấp hơn 1 bậc (VD: Intern vs Fresher)
                    else if (diff == -2)
                        totalFactor += 0.15;  // Thấp hơn 2 bậc (VD: Intern vs Junior)
                    else
                        totalFactor += 0.05;  // Thấp hơn ≥ 3 bậc (VD: Intern vs Middle/Senior)
                }

                levelScore = (totalFactor / matchedCount) * 15;
            }

            return Math.Round(coverageScore + levelScore, 1);
        }

        // ============================================================
        // MODULE 2: KINH NGHIỆM (Max 20đ)
        // Tách biệt Intern ≠ Fresher, xử lý tất cả biến thể
        // ============================================================
        private double CalculateExperienceScore(DevHub.Models.Candidate candidate, DevHub.Models.JobPost job)
        {
            string normalizedLevel = NormalizeExperienceLevel(job.ExperienceLevel);
            int cExp = candidate.ExperienceYears ?? 0;

            return normalizedLevel switch
            {
                "Intern" => cExp switch
                {
                    0 => 20,
                    1 => 15,
                    _ => 5        // Overqualified nặng
                },

                "Fresher" => cExp switch
                {
                    0 => 12,      // Intern → Fresher: underqualified nhẹ
                    1 => 20,      // Chính xác
                    2 => 15,
                    _ => 5        // Overqualified nặng
                },

                "Junior" => cExp switch
                {
                    0 => 5,       // Quá thiếu kinh nghiệm
                    1 or 2 => 20, // Chính xác
                    3 or 4 => 15,
                    _ => 5
                },

                "Middle" => cExp switch
                {
                    <= 0 => 0,    // Không đủ trình độ
                    1 => 3,       // Thiếu rất nhiều
                    2 => 8,       // Underqualified
                    3 or 4 => 20, // Chính xác
                    5 or 6 => 15,
                    _ => 5
                },

                "Senior" => cExp switch
                {
                    <= 1 => 0,    // Không đủ trình độ
                    2 => 3,
                    3 or 4 => 10,
                    _ => 20       // Chính xác (≥ 5 năm)
                },

                "Lead" or "Manager" => cExp switch
                {
                    < 3 => 0,
                    >= 3 and < 5 => 5,
                    >= 5 and < 8 => 12,
                    _ => 20
                },

                _ => cExp <= 1 ? 10 : 8  // Không rõ level
            };
        }

        // ============================================================
        // MODULE 3: HÌNH THỨC LÀM VIỆC (Max 20đ)
        // Remote ≠ Parttime! Remote = NƠI làm việc, Parttime = THỜI GIAN
        // ============================================================
        private double CalculateWorkingModelScore(DevHub.Models.Candidate candidate, DevHub.Models.JobPost job)
        {
            string cModel = NormalizeWorkingModel((candidate.PreferredWorkingModel ?? "").Trim().ToLower());
            string jModel = NormalizeWorkingModel((job.WorkingModel ?? "").Trim().ToLower());

            // Nếu ứng viên chưa chọn hình thức → mặc định
            if (string.IsNullOrEmpty(cModel))
                return 12;

            // Match hoàn hảo
            if (cModel == jModel)
                return 20;

            // Bảng tra cứu chi tiết
            return (cModel, jModel) switch
            {
                // === Ứng viên muốn PARTTIME ===
                ("parttime", "fulltime")         => 3,   // Xung đột thời gian nghiêm trọng
                ("parttime", "fulltime_onsite")  => 3,   // Tương tự
                ("parttime", "fulltime_remote")  => 5,   // Remote nhưng vẫn full-time → xung đột thời gian
                ("parttime", "remote")           => 5,   // Remote thường là full-time → xung đột thời gian
                ("parttime", "hybrid")           => 5,   // Hybrid thường là full-time
                ("parttime", "freelance")        => 15,  // Freelance linh hoạt thời gian

                // === Ứng viên muốn FULLTIME / FULLTIME_ONSITE ===
                ("fulltime", "parttime")         => 10,  // Full-time có thể nhận part-time
                ("fulltime", "fulltime_onsite")  => 20,  // Gần như giống
                ("fulltime", "fulltime_remote")  => 18,  // Vẫn full-time, chỉ khác nơi
                ("fulltime", "remote")           => 16,  // Remote thường full-time
                ("fulltime", "hybrid")           => 16,
                ("fulltime", "freelance")        => 10,

                ("fulltime_onsite", "fulltime")  => 20,
                ("fulltime_onsite", "parttime")  => 8,
                ("fulltime_onsite", "remote")    => 8,
                ("fulltime_onsite", "fulltime_remote") => 8,
                ("fulltime_onsite", "hybrid")    => 14,
                ("fulltime_onsite", "freelance") => 5,

                // === Ứng viên muốn FULLTIME_REMOTE ===
                ("fulltime_remote", "fulltime")  => 10,  // Có thể phải lên office
                ("fulltime_remote", "fulltime_onsite") => 5,
                ("fulltime_remote", "remote")    => 20,  // Remote = fulltime remote
                ("fulltime_remote", "parttime")  => 8,
                ("fulltime_remote", "hybrid")    => 12,
                ("fulltime_remote", "freelance") => 12,

                // === Ứng viên muốn REMOTE ===
                ("remote", "fulltime")           => 5,   // Bắt lên office → xung đột
                ("remote", "fulltime_onsite")    => 3,   // Bắt lên office
                ("remote", "fulltime_remote")    => 20,  // Remote!
                ("remote", "parttime")           => 8,
                ("remote", "hybrid")             => 12,
                ("remote", "freelance")          => 15,

                // === Ứng viên muốn HYBRID ===
                ("hybrid", "fulltime")           => 14,
                ("hybrid", "fulltime_onsite")    => 10,
                ("hybrid", "fulltime_remote")    => 16,
                ("hybrid", "remote")             => 18,
                ("hybrid", "parttime")           => 8,
                ("hybrid", "freelance")          => 10,

                // === Ứng viên muốn FREELANCE ===
                ("freelance", "fulltime")        => 5,
                ("freelance", "fulltime_onsite") => 3,
                ("freelance", "fulltime_remote") => 10,
                ("freelance", "remote")          => 12,
                ("freelance", "parttime")        => 12,
                ("freelance", "hybrid")          => 8,

                _ => 10
            };
        }

        // ============================================================
        // MODULE 4: MỨC LƯƠNG (Max 15đ)
        // Xét cả 2 chiều: quá rẻ hoặc quá đắt
        // ============================================================
        private double CalculateSalaryScore(DevHub.Models.Candidate candidate, DevHub.Models.JobPost job)
        {
            decimal cMin = candidate.ExpectedSalaryMin ?? 0;
            decimal cMax = candidate.ExpectedSalaryMax ?? 0;
            decimal jMin = job.SalaryMin ?? 0;
            decimal jMax = job.SalaryMax ?? 0;

            // Ứng viên chưa nhập lương
            if (cMin <= 0 && cMax <= 0) return 15;

            // Job không có thông tin lương
            if (jMin <= 0 && jMax <= 0) return 10;

            // Đảm bảo giá trị hợp lý
            if (cMax <= 0) cMax = cMin * 1.5m;
            if (jMax <= 0) jMax = jMin * 1.5m;

            // Check khoảng lương có giao nhau không
            bool hasOverlap = cMin <= jMax && jMin <= cMax;

            if (hasOverlap)
            {
                decimal overlapStart = Math.Max(cMin, jMin);
                decimal overlapEnd = Math.Min(cMax, jMax);
                decimal overlapRange = overlapEnd - overlapStart;
                decimal candidateRange = cMax - cMin;

                if (candidateRange > 0)
                {
                    double overlapRatio = (double)(overlapRange / candidateRange);
                    return overlapRatio >= 0.5 ? 15 : Math.Max(8, overlapRatio * 15);
                }
                return 15;
            }

            // Không giao nhau
            if (cMin > jMax)
            {
                // Ứng viên đòi cao hơn Job trả
                double ratio = (double)(jMax / cMin);
                if (ratio >= 0.8) return 10;
                if (ratio >= 0.6) return ratio * 15;
                return 0;
            }
            else
            {
                // Job trả cao hơn nhiều so với kỳ vọng ứng viên
                // → Ứng viên có thể chưa đủ level cho vị trí này
                double ratio = (double)(cMax / jMin);
                if (ratio >= 0.7) return 12;
                if (ratio >= 0.4) return 8;
                return 5;
            }
        }

        // ============================================================
        // MODULE 5: ĐỊA ĐIỂM (Max 10đ)
        // ============================================================
        private double CalculateLocationScore(DevHub.Models.Candidate candidate, DevHub.Models.JobPost job)
        {
            string jModelNorm = NormalizeWorkingModel((job.WorkingModel ?? "").Trim().ToLower());

            // Job Remote/Freelance → địa điểm không quan trọng
            if (jModelNorm is "remote" or "fulltime_remote" or "freelance")
                return 10;

            string cLoc = !string.IsNullOrWhiteSpace(candidate.PreferredLocation)
                            ? candidate.PreferredLocation.Trim().ToLower()
                            : (candidate.Address?.Trim().ToLower() ?? "");
            string jLoc = job.Location?.Trim().ToLower() ?? "";

            if (string.IsNullOrEmpty(cLoc) || string.IsNullOrEmpty(jLoc))
                return 5;

            var cParts = cLoc.Split(new[] { ',', '-' }, StringSplitOptions.RemoveEmptyEntries)
                             .Select(p => p.Trim()).ToList();
            var jParts = jLoc.Split(new[] { ',', '-' }, StringSplitOptions.RemoveEmptyEntries)
                             .Select(p => p.Trim()).ToList();

            if (cParts.Any(cp => jParts.Any(jp => jp == cp || jp.Contains(cp) || cp.Contains(jp))))
                return 10;

            return 0;
        }

        // ============================================================
        // HELPER: Chuẩn hoá ExperienceLevel
        // Xử lý TẤT CẢ kiểu viết tìm thấy trong seed data & form
        // ============================================================
        private static string NormalizeExperienceLevel(string? level)
        {
            if (string.IsNullOrWhiteSpace(level)) return "";

            string cleaned = level.Trim().ToLower()
                .Replace("-", "").Replace("_", "").Replace(" ", "");
            // cleaned: "intern","fresher","junior","mid","midlevel","middle",
            //          "juniormid","midsenior","senior","lead","manager"

            // Exact matches trước
            if (cleaned == "intern" || cleaned == "thựctập" || cleaned == "thuctap")
                return "Intern";

            if (cleaned == "fresher" || cleaned == "newgrad" || cleaned == "mớiratruong" || cleaned == "moiratrường")
                return "Fresher";

            if (cleaned.StartsWith("junior") && !cleaned.Contains("mid"))
                return "Junior";

            // Mid/Middle variations — phải check TRƯỚC "junior/mid"
            if (cleaned == "mid" || cleaned == "middle" || cleaned == "midlevel"
                || cleaned == "intermediate" || cleaned == "trungcấp")
                return "Middle";

            // Kết hợp: lấy level CAO hơn trong cặp (yêu cầu khắt khe hơn)
            if (cleaned == "juniormid" || cleaned.Contains("junior") && cleaned.Contains("mid"))
                return "Middle";  // Junior/Mid → tính theo Mid (khắt khe hơn)

            if (cleaned == "midsenior" || cleaned.Contains("mid") && cleaned.Contains("senior"))
                return "Senior";  // Mid/Senior → tính theo Senior

            if (cleaned.StartsWith("senior") || cleaned == "sr")
                return "Senior";

            if (cleaned == "lead" || cleaned == "techlead" || cleaned == "teamlead"
                || cleaned == "principal" || cleaned == "staff")
                return "Lead";

            if (cleaned == "manager" || cleaned == "director" || cleaned == "head")
                return "Manager";

            return "";
        }

        // ============================================================
        // HELPER: Chuyển Level thành Rank số để so sánh
        // Intern(0) < Fresher(1) < Junior(2) < Middle(3) < Senior(4) < Lead/Manager(5)
        // ============================================================
        private static int GetLevelRank(string? level)
        {
            string normalized = NormalizeExperienceLevel(level);
            return normalized switch
            {
                "Intern"  => 0,
                "Fresher" => 1,
                "Junior"  => 2,
                "Middle"  => 3,
                "Senior"  => 4,
                "Lead"    => 5,
                "Manager" => 5,
                _         => -1  // Không xác định → trả -1 để không cho điểm cao ngẫu nhiên
            };
        }

        // ============================================================
        // HELPER: Chuẩn hoá WorkingModel
        // Xử lý TẤT CẢ kiểu viết tìm thấy trong seed data & form
        // ============================================================
        private static string NormalizeWorkingModel(string model)
        {
            if (string.IsNullOrWhiteSpace(model)) return "";

            string cleaned = model.Trim().ToLower()
                .Replace("-", "").Replace("_", "").Replace(" ", "");
            // cleaned: "fulltimeonsite","fulltimeremote","fulltime","remote",
            //          "parttime","hybrid","freelance","onsite","contract"

            if (cleaned.Contains("fulltime") && cleaned.Contains("remote"))
                return "fulltime_remote";

            if (cleaned.Contains("fulltime") && (cleaned.Contains("onsite") || cleaned.Contains("office")))
                return "fulltime_onsite";

            if (cleaned.Contains("fulltime") || cleaned.Contains("full"))
                return "fulltime";

            if (cleaned.Contains("parttime") || cleaned.Contains("part"))
                return "parttime";

            if (cleaned.Contains("remote"))
                return "remote";

            if (cleaned.Contains("hybrid"))
                return "hybrid";

            if (cleaned.Contains("freelance") || cleaned.Contains("contract"))
                return "freelance";

            if (cleaned.Contains("onsite") || cleaned.Contains("office"))
                return "fulltime_onsite";

            return model;
        }
    }
}
