using DevHub.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace DevHub.Filters;

public class ModeratorTaskTypeFilter : IAsyncActionFilter
{
    private readonly string _requiredTaskType;
    private readonly IAssignModeratorService _assignService;

    public ModeratorTaskTypeFilter(string requiredTaskType, IAssignModeratorService assignService)
    {
        _requiredTaskType = requiredTaskType;
        _assignService    = assignService;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = context.HttpContext.User;
        if (user.Identity?.IsAuthenticated == true && user.IsInRole("Moderator"))
        {
            var idStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(idStr, out int moderatorId))
            {
                var assignedTaskType = await _assignService.GetTaskTypeAsync(moderatorId);

                if (string.IsNullOrEmpty(assignedTaskType))
                {
                    // Mod chưa được assign loại task nào => block
                    context.Result = new RedirectResult("/Home/Error403?reason=unassigned");
                    return;
                }

                if (assignedTaskType != _requiredTaskType)
                {
                    // Mod có loại task khác => block
                    context.Result = new RedirectResult($"/Home/Error403?reason=wrong_task_type&required={_requiredTaskType}&actual={assignedTaskType}");
                    return;
                }
            }
        }

        // Pass
        await next();
    }
}

public class ModeratorTaskTypeAttribute : TypeFilterAttribute
{
    public ModeratorTaskTypeAttribute(string requiredTaskType)
        : base(typeof(ModeratorTaskTypeFilter))
    {
        Arguments = new object[] { requiredTaskType };
    }
}
