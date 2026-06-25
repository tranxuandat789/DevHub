using System.ComponentModel.DataAnnotations;

namespace DevHub.ViewModels.Moderator
{
    public class UserManagementViewModel
    {
        public string? Search { get; set; } = "";

        public string? Role { get; set; } = "";

        public string? Sort { get; set; } = "";

        [Range(1, int.MaxValue, ErrorMessage = "Trang phải lớn hơn 0")]
        public int Page { get; set; } = 1;
    }
}
