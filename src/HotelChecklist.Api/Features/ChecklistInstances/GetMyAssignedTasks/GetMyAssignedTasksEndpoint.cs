using System.Security.Claims;
using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.ChecklistInstances.GetMyAssignedTasks;

public static class GetMyAssignedTasksEndpoint
{
    public static void MapGetMyAssignedTasks(this RouteGroupBuilder group)
    {
        group.MapGet("/my-tasks", async (
                DateOnly? date,
                ClaimsPrincipal user,
                IQueryHandler<GetMyAssignedTasksQuery, IReadOnlyList<MyAssignedTaskItem>> handler,
                CancellationToken cancellationToken) =>
            {
                var query = new GetMyAssignedTasksQuery(user.GetUserId(), date ?? DateOnly.FromDateTime(DateTime.UtcNow));
                var result = await handler.Handle(query, cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.AnyRole)
            .WithName("GetMyAssignedTasks")
            .Produces<IReadOnlyList<MyAssignedTaskItem>>();
    }
}
