using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevHub.Services.Interfaces;
using DevHub.Repositories.Interfaces;
using System.Threading.Tasks;
using System.Security.Claims;

namespace DevHub.Controllers.Recruiter
{
    [Route("Recruiter/[controller]")]
    [Authorize(Roles = "BUSINESS")]
    public class JobPostController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IRecruiterJobPostService _jobPostService;
        private readonly ICommonJobPositionRepository _positionRepo;
        private readonly ICommonTechnologyRepository _techRepo;

        public JobPostController(IAuthService authService, IRecruiterJobPostService jobPostService, ICommonJobPositionRepository positionRepo, ICommonTechnologyRepository techRepo)
        {
            _authService = authService;
            _jobPostService = jobPostService;
            _positionRepo = positionRepo;
            _techRepo = techRepo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            var email = User.FindFirstValue(System.Security.Claims.ClaimTypes.Email) ?? "";
            var dbUser = await _authService.FindUserByEmailAsync(email);
            if (dbUser == null || dbUser.Recruiter == null)
                return NotFound();

            var (canPost, hasPackage, postsRemaining, profileCompletion) = await _jobPostService.GetActivePackageInfoAsync(dbUser.Recruiter.RecruiterId);
            if (profileCompletion < 90)
            {
                TempData["Error"] = "Bạn cần hoàn thành đủ mục thông tin công ty";
                return RedirectToAction("Index", "Settings");
            }

            if (!hasPackage || postsRemaining <= 0)
            {
                TempData["Error"] = "Tài khoản không đủ lượt đăng. Vui lòng mua gói dịch vụ.";
                return Redirect("/recruiter/subscription");
            }

            ViewBag.Positions = await _positionRepo.GetAllActiveAsync();
            ViewBag.Techs = await _techRepo.GetAllActiveAsync();

            return View();
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DevHub.ViewModels.Recruiter.JobPostCreateViewModel dto)
        {
            var email = User.FindFirstValue(System.Security.Claims.ClaimTypes.Email) ?? "";
            var dbUser = await _authService.FindUserByEmailAsync(email);
            if (dbUser == null || dbUser.Recruiter == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Positions = await _positionRepo.GetAllActiveAsync();
                ViewBag.Techs = await _techRepo.GetAllActiveAsync();
                return View(dto);
            }

            try
            {
                var jobId = await _jobPostService.CreateJobPostAsync(dbUser.Recruiter.RecruiterId, dto);
                TempData["Success"] = "Bài đăng đã được tạo và đang chờ kiểm duyệt.";
                return RedirectToAction("Index");
            }
            catch (InvalidOperationException ex)
            {
                // Could be profile completeness or quota or validation
                TempData["Error"] = ex.Message;
                if (ex.Message.Contains("hoàn thành"))
                    return RedirectToAction("Index", "Settings");
                if (ex.Message.Contains("gói dịch vụ"))
                    return Redirect("/recruiter/subscription");

                ViewBag.Positions = await _positionRepo.GetAllActiveAsync();
                ViewBag.Techs = await _techRepo.GetAllActiveAsync();
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
        }

        [HttpGet("Edit")]
        public IActionResult Edit()
        {
            return View();
        }
    }
}
