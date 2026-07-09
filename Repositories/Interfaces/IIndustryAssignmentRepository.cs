using System.Collections.Generic;
using System.Threading.Tasks;
using DevHub.Models;

namespace DevHub.Repositories.Interfaces
{
    public interface IIndustryAssignmentRepository
    {
        // Lấy moderator_id phụ trách (task_type, industry). Null nếu chưa có.
        Task<int?> GetModeratorIdAsync(string taskType, string industry);

        // Lấy danh sách ngành hiện tại của 1 Moderator trong sub-role của họ
        Task<List<string>> GetIndustriesAsync(int moderatorId);

        // Lưu danh sách ngành mới (xóa cũ, thêm mới) cho Moderator trong sub-role đó
        Task SetIndustriesAsync(int moderatorId, string taskType, IEnumerable<string> industries, int assignedBy);

        // Kiểm tra ngành X đã có ai khác giữ chưa (để Admin biết mà cảnh báo)
        Task<int?> GetOwnerOfIndustryAsync(string taskType, string industry, int excludeModeratorId);

        // Lấy tất cả ngành phân công theo task_type (để Admin xem tổng quan)
        Task<List<ModeratorIndustryAssignment>> GetAllByTaskTypeAsync(string taskType);
    }
}
