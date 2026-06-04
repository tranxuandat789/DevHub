// =========================================================================
// Các thư viện, package phục vụ xử lý backend trang chủ (Homepage)
// Author: PhongDH
// Date: 31/06/2026
// =========================================================================
using System.Diagnostics;
using DevHub.Data;
using DevHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ItrecruitmentDbContext _context;

        public HomeController(ILogger<HomeController> logger, ItrecruitmentDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Hiển thị trang chủ với các dữ liệu nổi bật (việc làm, công ty, bài viết).
        /// </summary>
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách 6 việc làm đã được duyệt (Approved)
            // Sắp xếp ưu tiên theo điểm số (PriorityScore) giảm dần, sau đó theo ngày tạo mới nhất
            var featuredJobs = await _context.JobPosts
                .Include(j => j.Recruiter)
                .Include(j => j.Position)
                .Include(j => j.Teches)
                .Where(j => j.Status.ToLower() == "approved")
                .OrderByDescending(j => j.PriorityScore)
                .ThenByDescending(j => j.CreatedAt)
                .Take(6)
                .ToListAsync();

            // Lấy danh sách 8 công ty (nhà tuyển dụng) nổi bật đã được xác thực (IsVerified)
            // Dữ liệu bao gồm thông tin cơ bản và tổng số lượng việc làm đã được duyệt của từng công ty
            // Sắp xếp giảm dần theo số lượng việc làm
            var featuredCompanies = await _context.Recruiters
                .Where(r => r.IsVerified == true)
                .Select(r => new FeaturedCompanyViewModel
                {
                    RecruiterId = r.RecruiterId,
                    CompanyName = r.CompanyName,
                    CompanyLogoUrl = r.CompanyLogoUrl,
                    JobCount = _context.JobPosts.Count(j => j.RecruiterId == r.RecruiterId && j.Status.ToLower() == "approved")
                })
                .OrderByDescending(c => c.JobCount)
                .Take(8)
                .ToListAsync();

            // Lấy danh sách 5 bài viết cẩm nang/blog đã được xuất bản (IsPublished)
            // Sắp xếp giảm dần theo ngày xuất bản mới nhất
            var featuredBlogs = await _context.BlogPosts
                .Where(b => b.IsPublished == true)
                .OrderByDescending(b => b.PublishedAt)
                .Take(5)
                .ToListAsync();

            // Khởi tạo ViewModel chứa các dữ liệu hiển thị cho trang chủ
            var viewModel = new HomeViewModel
            {
                FeaturedJobs = featuredJobs,
                FeaturedCompanies = featuredCompanies,
                FeaturedBlogs = featuredBlogs
            };

            return View(viewModel);
        }

        public IActionResult Employer()
        {
            return View("~/Views/Recruiter/Home/Index.cshtml");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Route("Home/Error404")]
        public IActionResult Error404()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // --- Temporary Route to preview UI without creating a new Controller ---
        [Route("JobApproval")]
        public IActionResult JobApprovalPreview()
        {
            return View("~/Views/Moderator/JobApproval/Index.cshtml");
        }

        [Route("ModeratorBlog")]
        public IActionResult ModeratorBlogPreview()
        {
            return View("~/Views/Moderator/Blog/Index.cshtml");
        }

        [Route("ModeratorBlog/Create")]
        public IActionResult ModeratorBlogCreatePreview()
        {
            return View("~/Views/Moderator/Blog/Create.cshtml");
        }

        [Route("ModeratorUser")]
        public IActionResult ModeratorUserPreview()
        {
            return View("~/Views/Moderator/ModeratorUserManagement/Index.cshtml");
        }

        [Route("ModeratorPackage")]
        public IActionResult ModeratorPackagePreview()
        {
            return View("~/Views/Moderator/PackageManagement/Index.cshtml");
        }

        [Route("ModeratorPackage/Create")]
        public IActionResult ModeratorPackageCreatePreview()
        {
            return View("~/Views/Moderator/PackageManagement/Create.cshtml");
        }

        [Route("SystemLog")]
        public IActionResult SystemLogPreview()
        {
            return View("~/Views/Moderator/SystemLog/Index.cshtml");
        }

        [Route("ModeratorVoucher")]
        public IActionResult ModeratorVoucherPreview()
        {
            return View("~/Views/Moderator/Voucher/Index.cshtml");
        }

        [Route("ModeratorVoucher/Create")]
        public IActionResult ModeratorVoucherCreatePreview()
        {
            return View("~/Views/Moderator/Voucher/Create.cshtml");
        }

        [Route("ModeratorPosition")]
        public IActionResult ModeratorPositionPreview()
        {
            return View("~/Views/Moderator/JobPosition/Index.cshtml");
        }

        [Route("ModeratorPosition/Create")]
        public IActionResult ModeratorPositionCreatePreview()
        {
            return View("~/Views/Moderator/JobPosition/Create.cshtml");
        }

        [Route("ReviewApproval")]
        public IActionResult ReviewApprovalPreview()
        {
            return View("~/Views/Moderator/ReviewApproval/Index.cshtml");
        }



        [Route("AdminUser")]
        public IActionResult AdminUserPreview()
        {
            return View("~/Views/Admin/AdminUserManagement/Index.cshtml");
        }

        [Route("AdminDashboard")]
        public IActionResult AdminDashboardPreview()
        {
            return View("~/Views/Admin/AdminDashboard/Index.cshtml");
        }

        [Route("AdminTransaction")]
        public IActionResult AdminTransactionPreview()
        {
            return View("~/Views/Admin/AdminTransaction/Index.cshtml");
        }

        [Route("AdminSubscription")]
        public IActionResult AdminSubscriptionPreview()
        {
            return View("~/Views/Admin/AdminSubscription/Index.cshtml");
        }

        [Route("AdminModerator")]
        public IActionResult AdminModeratorPreview()
        {
            return View("~/Views/Admin/AdminModerator/Index.cshtml");
        }

        [Route("AdminModerator/Create")]
        public IActionResult AdminModeratorCreatePreview()
        {
            return View("~/Views/Admin/AdminModerator/Create.cshtml");
        }
    }
}
