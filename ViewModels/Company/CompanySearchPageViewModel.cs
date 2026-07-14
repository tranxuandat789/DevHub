using System.Collections.Generic;
using DevHub.Models;

namespace DevHub.ViewModels.Company
{
    public class CompanySearchPageViewModel
    {
        public List<CompanySearchItemViewModel> Companies { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; } = 5; // Updated to 5 as requested
        public int TotalPages { get; set; }
        public string? Name { get; set; }
        public string? Industry { get; set; }
        public string? Address { get; set; }
        
        // New search criteria
        public string? SearchTerm { get; set; }
        public string? SortOrder { get; set; }
        
        // Sidebar data
        public List<CommonTechnology> AvailableTechs { get; set; } = new();
        public List<CommonJobPosition> AvailablePositions { get; set; } = new();
        
        // Selected filters
        public List<int> SelectedTechs { get; set; } = new();
        public List<int> SelectedPositions { get; set; } = new();
    }
}
