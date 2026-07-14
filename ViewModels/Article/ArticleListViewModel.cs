using DevHub.Models;

namespace DevHub.ViewModels.Articles
{
    public class ArticleListViewModel
    {
        // Danh sách bài viết trang hiện tại
        public List<Article> Articles { get; set; } = new();

        // Sidebar: top công ty theo số bài
        public List<(int CompanyId, string CompanyName, string? LogoUrl, int Count)> CompanyFilters { get; set; } = new();

        // Input từ query string
        public string? Search { get; set; }
        public int? CompanyId { get; set; }
        public string Sort { get; set; } = "newest"; // newest | oldest

        // Pagination
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
