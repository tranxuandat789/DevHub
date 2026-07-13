using System.Collections.Generic;
using DevHub.Models;

namespace DevHub.ViewModels.Candidate;

public class MyReviewsViewModel
{
    public List<ReviewCompany> Reviews { get; set; } = new();
    public int Page { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
}
