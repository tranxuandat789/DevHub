using DevHub.Models;
using DevHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DevHub.Controllers.Moderator
{
    [Route("moderator/vouchers")]
    [Authorize(Roles = "Moderator,ADMIN,MODERATOR")]
    public class VoucherController : Controller
    {
        private readonly IPromotionService _promotionService;

        public VoucherController(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        [HttpGet("")]
        [HttpGet("/ModeratorVoucher")]
        public async Task<IActionResult> Index(int page = 1, string sortBy = "date_desc", string? keyword = null)
        {
            int pageSize = 10;
            var (items, totalCount) = await _promotionService.GetAllVouchersAsync(page, pageSize, sortBy, keyword);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (totalCount + pageSize - 1) / pageSize;
            ViewBag.SortBy = sortBy;
            ViewBag.Keyword = keyword;

            return View("~/Views/Moderator/Voucher/Index.cshtml", items);
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var promotion = await _promotionService.GetByIdAsync(id);
            if (promotion == null)
                return NotFound();

            return View("~/Views/Moderator/Voucher/Details.cshtml", promotion);
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            return View("~/Views/Moderator/Voucher/Create.cshtml", new Promotion { IsActive = true });
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(Promotion promotion)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Moderator/Voucher/Create.cshtml", promotion);

            var (success, created, errorMessage) = await _promotionService.CreateAsync(promotion);

            if (success)
            {
                TempData["SuccessMessage"] = "Voucher created successfully.";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", errorMessage ?? "An error occurred.");
            return View("~/Views/Moderator/Voucher/Create.cshtml", promotion);
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var promotion = await _promotionService.GetByIdAsync(id);
            if (promotion == null)
                return NotFound();

            return View("~/Views/Moderator/Voucher/Edit.cshtml", promotion);
        }

        [HttpPost("edit/{id}")]
        public async Task<IActionResult> Edit(int id, Promotion promotion)
        {
            if (id != promotion.PromotionId)
                return BadRequest();

            if (!ModelState.IsValid)
                return View("~/Views/Moderator/Voucher/Edit.cshtml", promotion);

            var (success, updated, errorMessage) = await _promotionService.UpdateAsync(promotion);

            if (success)
            {
                TempData["SuccessMessage"] = "Voucher updated successfully.";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", errorMessage ?? "An error occurred.");
            return View("~/Views/Moderator/Voucher/Edit.cshtml", promotion);
        }

        [HttpPost("deactivate/{id}")]
        public async Task<IActionResult> Deactivate(int id)
        {
            bool deactivated = await _promotionService.DeactivateAsync(id);
            if (deactivated)
            {
                TempData["SuccessMessage"] = "Vô hiệu hóa mã giảm giá thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể vô hiệu hóa mã giảm giá hoặc mã không tồn tại.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost("activate/{id}")]
        public async Task<IActionResult> Activate(int id)
        {
            bool activated = await _promotionService.ActivateAsync(id);
            if (activated)
            {
                TempData["SuccessMessage"] = "Kích hoạt mã giảm giá thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể kích hoạt mã giảm giá hoặc mã không tồn tại.";
            }
            return RedirectToAction("Index");
        }

        [HttpGet("generate-code")]
        public IActionResult GenerateCode()
        {
            var code = _promotionService.GeneratePromoCode();
            return Json(new { code });
        }
    }
}
