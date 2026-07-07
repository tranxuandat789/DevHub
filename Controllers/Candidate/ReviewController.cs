using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DevHub.Models;
using DevHub.Services.Interfaces;
using DevHub.ViewModels.Candidate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Candidate
{
    [Authorize(Roles = "CANDIDATE,Candidate")]
    public class ReviewController : Controller
    {
        private readonly IReviewCompanyService _reviewService;
        private readonly ICompanyService _companyService;

        public ReviewController(IReviewCompanyService reviewService, ICompanyService companyService)
        {
            _reviewService = reviewService;
            _companyService = companyService;
        }

        [HttpGet("candidate/reviews")]
        public async Task<IActionResult> Index(int page = 1)
        {
            var candidateId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            int pageSize = 5;

            var (items, totalCount) = await _reviewService.GetByCandidateAsync(candidateId, page, pageSize);

            var model = new MyReviewsViewModel
            {
                Reviews = items,
                Page = page,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            ViewData["ActiveMenu"] = "Reviews";
            return View("~/Views/Candidate/Review/Index.cshtml", model);
        }

        [HttpGet("candidate/reviews/{id}")]
        public async Task<IActionResult> Detail(int id)
        {
            var candidateId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var review = await _reviewService.GetByIdAsync(id);

            if (review == null || review.CandidateId != candidateId)
            {
                return NotFound();
            }

            ViewData["ActiveMenu"] = "Reviews";
            return View("~/Views/Candidate/Review/Detail.cshtml", review);
        }

        [HttpGet("Companies/{companyId}/WriteReview")]
        public async Task<IActionResult> Create(int companyId)
        {
            var candidateId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
            var companyDetails = await _companyService.GetCompanyDetailsAsync(companyId);
            if (companyDetails == null) return NotFound();

            var existingReview = await _reviewService.GetByCandidateAndCompanyAsync(candidateId, companyId);

            var model = new WriteReviewViewModel
            {
                CompanyId = companyId,
                CompanyName = companyDetails.Company.CompanyName,
                AlreadyReviewed = existingReview != null,
                ExistingReview = existingReview
            };

            return View("~/Views/Candidate/Review/Create.cshtml", model);
        }

        [HttpPost("Companies/{companyId}/WriteReview")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int companyId, WriteReviewViewModel model)
        {
            var candidateId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
            if (!ModelState.IsValid)
            {
                var companyDetails = await _companyService.GetCompanyDetailsAsync(companyId);
                if (companyDetails != null)
                {
                    model.CompanyName = companyDetails.Company.CompanyName;
                }
                return View("~/Views/Candidate/Review/Create.cshtml", model);
            }

            var review = new ReviewCompany
            {
                CandidateId = candidateId,
                CompanyId = companyId,
                Rating = model.Rating,
                SalaryRating = model.SalaryRating,
                TrainingRating = model.TrainingRating,
                CareRating = model.CareRating,
                CultureRating = model.CultureRating,
                WorkspaceRating = model.WorkspaceRating,
                Pros = model.Pros,
                Cons = model.Cons,
                OtPolicy = model.OtPolicy,
                Recommend = model.Recommend
            };

            var (success, message) = await _reviewService.CreateReviewAsync(review);
            
            if (success)
            {
                TempData["SuccessMessage"] = message;
                return RedirectToAction("Details", "Companies", new { id = companyId });
            }
            else
            {
                ModelState.AddModelError("", message);
                var companyDetails = await _companyService.GetCompanyDetailsAsync(companyId);
                if (companyDetails != null)
                {
                    model.CompanyName = companyDetails.Company.CompanyName;
                }
                return View("~/Views/Candidate/Review/Create.cshtml", model);
            }
        }

        [HttpGet("candidate/reviews/{id}/edit")]
        public async Task<IActionResult> Edit(int id)
        {
            var candidateId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var review = await _reviewService.GetByIdAsync(id);

            if (review == null || review.CandidateId != candidateId)
            {
                return NotFound();
            }

            if (review.Status == "APPROVED")
            {
                TempData["ErrorMessage"] = "Không thể sửa đánh giá đã được duyệt.";
                return RedirectToAction("Detail", new { id = id });
            }

            var model = new WriteReviewViewModel
            {
                CompanyId = review.CompanyId,
                CompanyName = review.Company.CompanyName,
                AlreadyReviewed = true,
                ExistingReview = review,
                Rating = review.Rating,
                SalaryRating = review.SalaryRating,
                TrainingRating = review.TrainingRating,
                CareRating = review.CareRating,
                CultureRating = review.CultureRating,
                WorkspaceRating = review.WorkspaceRating,
                Pros = review.Pros,
                Cons = review.Cons,
                OtPolicy = review.OtPolicy,
                Recommend = review.Recommend
            };

            ViewData["ActiveMenu"] = "Reviews";
            return View("~/Views/Candidate/Review/Edit.cshtml", model);
        }

        [HttpPost("candidate/reviews/{id}/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WriteReviewViewModel model)
        {
            var candidateId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
            if (!ModelState.IsValid)
            {
                ViewData["ActiveMenu"] = "Reviews";
                return View("~/Views/Candidate/Review/Edit.cshtml", model);
            }

            var review = new ReviewCompany
            {
                ReviewId = id,
                CompanyId = model.CompanyId,
                Rating = model.Rating,
                SalaryRating = model.SalaryRating,
                TrainingRating = model.TrainingRating,
                CareRating = model.CareRating,
                CultureRating = model.CultureRating,
                WorkspaceRating = model.WorkspaceRating,
                Pros = model.Pros,
                Cons = model.Cons,
                OtPolicy = model.OtPolicy,
                Recommend = model.Recommend
            };

            var (success, message) = await _reviewService.UpdateReviewAsync(review, candidateId);
            
            if (success)
            {
                TempData["SuccessMessage"] = message;
                return RedirectToAction("Detail", new { id = id });
            }
            else
            {
                ModelState.AddModelError("", message);
                ViewData["ActiveMenu"] = "Reviews";
                return View("~/Views/Candidate/Review/Edit.cshtml", model);
            }
        }
    }
}