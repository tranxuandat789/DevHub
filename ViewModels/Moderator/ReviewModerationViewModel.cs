using System;
using System.Collections.Generic;
using DevHub.Models;

namespace DevHub.ViewModels.Moderator;

public class ReviewModerationViewModel
{
    public List<ReviewCompany> Reviews { get; set; } = new();
    public string? StatusFilter { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; } = 10;
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public int FromItem { get; set; }
    public int ToItem { get; set; }
}
