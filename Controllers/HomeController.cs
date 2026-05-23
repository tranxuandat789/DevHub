using System.Diagnostics;
using DevHub.Models;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Employer()
        {
            return View();
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
