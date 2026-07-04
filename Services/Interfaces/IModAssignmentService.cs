using System.Threading.Tasks;

namespace DevHub.Services.Interfaces;

public interface IModAssignmentService
{
    // Returns an AdminId (Moderator) assigned to handle the given ServiceId.
    // Picks one based on least loaded or round-robin.
    // Returns null if no mod is assigned to that tier.
    Task<int?> GetAssignedModeratorAsync(int serviceId);
    Task AssignModeratorToTierAsync(int moderatorId, int serviceId);
    Task RemoveModeratorFromTierAsync(int moderatorId, int serviceId);
}
