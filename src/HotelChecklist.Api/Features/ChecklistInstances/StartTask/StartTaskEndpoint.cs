using System.Security.Claims;
using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.ChecklistInstances.StartTask;

public static class StartTaskEndpoint
{
    public static void MapStartTask(this RouteGroupBuilder group)
    {
        group.MapPost("/{instanceId:guid}/tasks/{taskExecutionId:guid}/start", async (
                Guid instanceId,
                Guid taskExecutionId,
                ClaimsPrincipal user,
                ICommandHandler<StartTaskCommand, StartTaskResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new StartTaskCommand(instanceId, taskExecutionId, user.GetUserId(), user.IsSupervisorOrAbove());
                var result = await handler.Handle(command, cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.AnyRole)
            .WithName("StartTask")
            .Produces<StartTaskResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status403Forbidden);
    }
}
