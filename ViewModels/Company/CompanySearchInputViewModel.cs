using System.Collections.Generic;

namespace DevHub.ViewModels.Company
{
    public class CompanySearchInputViewModel
    {
        public string? SearchTerm { get; set; }
        public List<int> SelectedTechs { get; set; } = new();
        public List<int> SelectedPositions { get; set; } = new();
        public string? SortOrder { get; set; }
        public int Page { get; set; } = 1;
    }
}
