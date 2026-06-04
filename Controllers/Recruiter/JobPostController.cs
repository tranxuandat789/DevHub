//AnhPT-04/06/2026
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevHub.Services.Interfaces;
using DevHub.Repositories.Interfaces;
using DevHub.ViewModels.Recruiter;
using System.Threading.Tasks;
using System.Security.Claims;

namespace DevHub.Controllers.Recruiter
{
    [Route("Recruiter/[controller]")]
    [Authorize(Roles = "RECRUITER")]
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

        // Management grid: check profile completeness, then render filtered/paginated posts.
        [HttpGet]
        public async Task<IActionResult> Index(string? q, string? status, int page = 1)
        {
            var email = User.FindFirstValue(System.Security.Claims.ClaimTypes.Email) ?? "";
            var dbUser = await _authService.FindUserByEmailAsync(email);
            if (dbUser == null || dbUser.Recruiter == null)
                return NotFound();

            // Check: company profile must be >= 90% complete to access the management page.
            if ((dbUser.Recruiter.ProfileCompletion ?? 0) < 90)
            {
                ViewBag.ProfileIncomplete = true;
                return View(new JobPostManageViewModel());
            }

            var vm = await _jobPostService.GetManagedJobPostsAsync(dbUser.Recruiter.RecruiterId, q, status, page, 10);
            return View(vm);
        }

        // Read-only detail for any post owned by the recruiter. If the post is editable, show the Edit button.
        [HttpGet("Detail/{id:int}")]
        public async Task<IActionResult> Detail(int id, string? q, string? status, int page = 1)
        {
            var email = User.FindFirstValue(System.Security.Claims.ClaimTypes.Email) ?? "";
            var dbUser = await _authService.FindUserByEmailAsync(email);
            if (dbUser == null || dbUser.Recruiter == null)
                return NotFound();

            var job = await _jobPostService.GetJobPostDetailAsync(dbUser.Recruiter.RecruiterId, id);
            if (job == null)
            {
                TempData["Error"] = "Tin tuyển dụng không tồn tại.";
                return RedirectToAction("Index", new { q, status, page });
            }

            ViewBag.Q = q; ViewBag.Status = status; ViewBag.Page = page;
            return View(job);
        }

        // Render the create form after enforcing profile completeness and remaining quota.
        [HttpGet("Create")]
        public async Task<IActionResult> Create(string? q, string? status, int page = 1)
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
            ViewBag.Q = q; ViewBag.Status = status; ViewBag.Page = page;

            return View();
        }

        // Validate and create a new post, service handles quota decrement + moderator notify.
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DevHub.ViewModels.Recruiter.JobPostCreateViewModel vm, string? q, string? status, int page = 1)
        {
            var email = User.FindFirstValue(System.Security.Claims.ClaimTypes.Email) ?? "";
            var dbUser = await _authService.FindUserByEmailAsync(email);
            if (dbUser == null || dbUser.Recruiter == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Positions = await _positionRepo.GetAllActiveAsync();
                ViewBag.Techs = await _techRepo.GetAllActiveAsync();
                ViewBag.Q = q; ViewBag.Status = status; ViewBag.Page = page;
                return View(vm);
            }

            try
            {
                var jobId = await _jobPostService.CreateJobPostAsync(dbUser.Recruiter.RecruiterId, vm);
                TempData["Success"] = "Bài đăng đã được tạo và đang chờ kiểm duyệt.";
                return RedirectToAction("Index", new { q, status, page });
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
                ViewBag.Q = q; ViewBag.Status = status; ViewBag.Page = page;
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(vm);
            }
        }

        // Render the edit form, only APPROVED/REJECTED posts are editable.
        [HttpGet("Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id, string? q, string? status, int page = 1)
        {
            var email = User.FindFirstValue(System.Security.Claims.ClaimTypes.Email) ?? "";
            var dbUser = await _authService.FindUserByEmailAsync(email);
            if (dbUser == null || dbUser.Recruiter == null)
                return NotFound();

            //Check job status that only APPROVED/REJECTED post can be edited.
            var job = await _jobPostService.GetEditableJobPostAsync(dbUser.Recruiter.RecruiterId, id);
            if (job == null)
            {
                TempData["Error"] = "Tin tuyển dụng không tồn tại hoặc không thể chỉnh sửa (chỉ tin Đã duyệt/Bị từ chối mới sửa được).";
                return RedirectToAction("Index", new { q, status, page });
            }

            // Load positions and techs for dropdowns, also pass current tech-stack and status for displaying in the view.
            ViewBag.Positions = await _positionRepo.GetAllActiveAsync();
            ViewBag.Techs = await _techRepo.GetAllActiveAsync();
            ViewBag.JobId = job.JobId;
            ViewBag.SelectedTechIds = job.Teches.Select(t => t.TechId).ToList();
            ViewBag.CurrentStatus = job.Status;
            ViewBag.Q = q; ViewBag.Status = status; ViewBag.Page = page;

            var vm = new JobPostCreateViewModel
            {
                Title = job.Title,
                PositionId = job.PositionId,
                TechnologyIds = job.Teches.Select(t => t.TechId).ToList(),
                Description = job.Description ?? "",
                Requirement = job.Requirement ?? "",
                Benefit = job.Benefit ?? "",
                ExperienceLevel = job.ExperienceLevel ?? "",
                Location = job.Location,
                WorkingModel = job.WorkingModel,
                SalaryMin = job.SalaryMin ?? 0,
                SalaryMax = job.SalaryMax ?? 0,
                HiringQuota = job.HiringQuota ?? 1,
                Deadline = job.Deadline ?? DateOnly.FromDateTime(DateTime.Today),
                Skill = job.Skill
            };
            return View(vm);
        }

        // Persist edits, service resubmits the post as PENDING for moderator re-review.
        [HttpPost("Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, JobPostCreateViewModel vm, string? q, string? status, int page = 1)
        {
            //Get current user and check if they have a recruiter profile
            var email = User.FindFirstValue(System.Security.Claims.ClaimTypes.Email) ?? "";
            var dbUser = await _authService.FindUserByEmailAsync(email);
            if (dbUser == null || dbUser.Recruiter == null)
                return NotFound();

            // Validate the input model
            if (!ModelState.IsValid)
            {
                ViewBag.Positions = await _positionRepo.GetAllActiveAsync();
                ViewBag.Techs = await _techRepo.GetAllActiveAsync();
                ViewBag.JobId = id;
                ViewBag.SelectedTechIds = vm.TechnologyIds ?? new List<int>();
                ViewBag.Q = q; ViewBag.Status = status; ViewBag.Page = page;
                return View(vm);
            }

            try
            {
                //update successfully, redirect to index keeping the original filters.
                await _jobPostService.UpdateJobPostAsync(dbUser.Recruiter.RecruiterId, id, vm);
                TempData["Success"] = "Cập nhật tin tuyển dụng thành công. Tin đang chờ kiểm duyệt lại.";
                return RedirectToAction("Index", new { q, status, page });
            }
            catch (KeyNotFoundException)
            {
                //Exception: Invalid Job post
                TempData["Error"] = "Tin tuyển dụng không tồn tại.";
                return RedirectToAction("Index", new { q, status, page });
            }
            catch (InvalidOperationException ex)
            {
                //Exception: business rule violation, ex: profile completeness or invalid status transition.
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.Q = q; ViewBag.Status = status; ViewBag.Page = page;
                ViewBag.Positions = await _positionRepo.GetAllActiveAsync();
                ViewBag.Techs = await _techRepo.GetAllActiveAsync();
                ViewBag.JobId = id;
                ViewBag.SelectedTechIds = vm.TechnologyIds ?? new List<int>();
                return View(vm);
            }
        }

        // Permanently delete a Rejected/Closed post after the confirm modal; service purges dependents.
        [HttpPost("Delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string? q, string? status, int page = 1)
        {
            //Get current user and check authorization.
            var email = User.FindFirstValue(System.Security.Claims.ClaimTypes.Email) ?? "";
            var dbUser = await _authService.FindUserByEmailAsync(email);
            if (dbUser == null || dbUser.Recruiter == null)
                return NotFound();

            try
            {
                //Delete and handle cascade deletes successfully, redirect to index.
                await _jobPostService.DeleteJobPostAsync(dbUser.Recruiter.RecruiterId, id);
                TempData["Success"] = "Đã xóa tin tuyển dụng.";
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "Tin tuyển dụng không tồn tại.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (Exception)
            {
                //fail to delete due to unexpected error, may be due to related data could not be deleted (ex: FK constraint).
                TempData["Error"] = "Không thể xóa tin do còn dữ liệu liên quan. Vui lòng thử lại hoặc liên hệ quản trị.";
            }
            // Keep the originating list filters after delete.
            return RedirectToAction("Index", new { q, status, page });
        }
    }
}
