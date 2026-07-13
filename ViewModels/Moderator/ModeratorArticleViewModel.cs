using DevHub.Models;
using System.Collections.Generic;

namespace DevHub.ViewModels.Moderator
{
    public class ModeratorArticleViewModel
    {
        public IEnumerable<DevHub.Models.Article> Articles { get; set; } = new List<DevHub.Models.Article>();
        public string? Keyword { get; set; }
        public string? DateFrom { get; set; }
        public string? Status { get; set; }
        public string? CompanyName { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int FromItem { get; set; }
        public int ToItem { get; set; }
    }
}
