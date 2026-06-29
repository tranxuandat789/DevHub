//AnhPT-18/06/2026
using DevHub.Services.Interfaces;
using DevHub.ViewModels.Recruiter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DevHub.Controllers.Recruiter
{
    [Route("Recruiter/[controller]")]
    [Authorize(Roles = "RECRUITER")]
    public class RecruiterApplicationController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IRecruiterApplicationService _appService;

        public RecruiterApplicationController(IAuthService authService, IRecruiterApplicationService appService)
        {
            _authService = authService;
            _appService = appService;
        }

        // Resolves the logged-in recruiter id; returns null when not found.
        private async Task<int?> GetRecruiterIdAsync()
        {
            var email = User.FindFirstValue(ClaimTypes.Email) ?? "";
            var dbUser = await _authService.FindUserByEmailAsync(email);
            return dbUser?.Recruiter?.RecruiterId;
        }

        // View Applicants of one APPROVED/CLOSED job owned by the recruiter.
        [HttpGet]
        public async Task<IActionResult> Index(int? jobId, ApplicantFilter filter)
        {
            var recruiterId = await GetRecruiterIdAsync();
            if (recruiterId == null) return NotFound();

            if (jobId == null)
                return RedirectToAction(nameof(All));

            var vm = await _appService.GetJobApplicantsAsync(recruiterId.Value, jobId.Value, filter);
            if (vm == null)
            {
                TempData["Error"] = "Không tìm thấy tin tuyển dụng hoặc tin chưa được duyệt.";
                return RedirectToAction("Index", "JobPost");
            }

            return View("~/Views/Recruiter/RecruiterApplication/Index.cshtml", vm);
        }

        // Cross-job lay-out: all applicants across the recruiter's jobs.
        [HttpGet("All")]
        public async Task<IActionResult> All(ApplicantFilter filter)
        {
            var recruiterId = await GetRecruiterIdAsync();
            if (recruiterId == null) return NotFound();

            var vm = await _appService.GetAllApplicantsAsync(recruiterId.Value, filter);
            return View("~/Views/Recruiter/RecruiterApplication/Index.cshtml", vm);
        }

        // Get/View candidate profile + CV.
        [HttpGet("Details/{applicationId:int}")]
        public async Task<IActionResult> Details(int applicationId)
        {
            var recruiterId = await GetRecruiterIdAsync();
            if (recruiterId == null) return NotFound();

            var vm = await _appService.GetCandidateProfileAsync(recruiterId.Value, applicationId);
            if (vm == null)
            {
                TempData["Error"] = "Không tìm thấy hồ sơ ứng viên.";
                return RedirectToAction("Index", "JobPost");
            }

            return View("~/Views/Recruiter/RecruiterApplication/Details.cshtml", vm);
        }

        // Candidate Profile: full application history of one candidate across all the recruiter's jobs.
        [HttpGet("Candidate/{candidateId:int}")]
        public async Task<IActionResult> CandidateProfile(int candidateId)
        {
            var recruiterId = await GetRecruiterIdAsync();
            if (recruiterId == null) return NotFound();

            var vm = await _appService.GetCandidateProfileHistoryAsync(recruiterId.Value, candidateId);
            if (vm == null)
            {
                TempData["Error"] = "Không tìm thấy hồ sơ ứng viên hoặc ứng viên chưa ứng tuyển vào công ty bạn.";
                return RedirectToAction(nameof(All));
            }

            return View("~/Views/Recruiter/RecruiterApplication/CandidateProfile.cshtml", vm);
        }

        // Approve an application.
        [HttpPost("Approve/{applicationId:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int applicationId)
        {
            var recruiterId = await GetRecruiterIdAsync();
            if (recruiterId == null) return NotFound();

            var (success, message) = await _appService.ApproveAsync(recruiterId.Value, applicationId);
            TempData[success ? "Success" : "Error"] = message;
            return RedirectToAction(nameof(Details), new { applicationId });
        }

        // Rejecting an application.
        [HttpPost("Reject/{applicationId:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int applicationId)
        {
            var recruiterId = await GetRecruiterIdAsync();
            if (recruiterId == null) return NotFound();

            var (success, message) = await _appService.RejectAsync(recruiterId.Value, applicationId);
            TempData[success ? "Success" : "Error"] = message;
            return RedirectToAction(nameof(Details), new { applicationId });
        }
    }
}
