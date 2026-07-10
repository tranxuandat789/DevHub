using DevHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DevHub.Controllers.Recruiter
{
    [Route("Recruiter/[controller]")]
    [Authorize(Roles = "RECRUITER")]
    public class RecruiterInterviewController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IInterviewService _interviewService;

        public RecruiterInterviewController(IAuthService authService, IInterviewService interviewService)
        {
            _authService = authService;
            _interviewService = interviewService;
        }

        private async Task<int?> GetRecruiterIdAsync()
        {
            var email = User.FindFirstValue(ClaimTypes.Email) ?? "";
            var dbUser = await _authService.FindUserByEmailAsync(email);
            return dbUser?.Recruiter?.RecruiterId;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Views/Recruiter/RecruiterInterview/Index.cshtml");
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Recruiter/RecruiterInterview/Create.cshtml");
        }
        
        [HttpGet("Edit/{id?}")]
        public IActionResult Edit(int? id)
        {
            return View("~/Views/Recruiter/RecruiterInterview/Edit.cshtml");
        }
        
        [HttpGet("Details/{id?}")]
        public IActionResult Details(int? id)
        {
            return View("~/Views/Recruiter/RecruiterInterview/Details.cshtml");
        }

        public class InterviewDto
        {
            public int ApplicationId { get; set; }
            public DateTime InterviewDate { get; set; }
            public int DurationMinutes { get; set; }
            public string InterviewType { get; set; } = "";
            public string LocationOrLink { get; set; } = "";
            public string? Notes { get; set; }
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreatePost([FromBody] InterviewDto model)
        {
            var recruiterId = await GetRecruiterIdAsync();
            if (recruiterId == null) return Unauthorized(new { success = false, message = "Bạn không có quyền thực hiện thao tác này." });

            if (model.ApplicationId <= 0 || string.IsNullOrEmpty(model.InterviewType) || string.IsNullOrEmpty(model.LocationOrLink))
                return BadRequest(new { success = false, message = "Vui lòng nhập đầy đủ thông tin bắt buộc." });

            try
            {
                var interview = await _interviewService.CreateInterviewAsync(recruiterId.Value, model.ApplicationId, model.InterviewDate, model.InterviewType, model.LocationOrLink, model.Notes);
                return Ok(new { success = true, message = "Tạo lịch phỏng vấn thành công.", interviewId = interview.InterviewId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> EditPost(int id, [FromBody] InterviewDto model)
        {
            var recruiterId = await GetRecruiterIdAsync();
            if (recruiterId == null) return Unauthorized(new { success = false, message = "Bạn không có quyền thực hiện thao tác này." });

            if (string.IsNullOrEmpty(model.InterviewType) || string.IsNullOrEmpty(model.LocationOrLink))
                return BadRequest(new { success = false, message = "Vui lòng nhập đầy đủ thông tin bắt buộc." });

            try
            {
                await _interviewService.UpdateInterviewAsync(recruiterId.Value, id, model.InterviewDate, model.InterviewType, model.LocationOrLink, model.Notes);
                return Ok(new { success = true, message = "Cập nhật lịch phỏng vấn thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
