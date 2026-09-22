using System.Security.Claims;
using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.ChecklistInstances.AssignTask;

public static class AssignTaskEndpoint
{
    public static void MapAssignTask(this RouteGroupBuilder group)
    {
        group.MapPost("/{instanceId:guid}/tasks/{taskExecutionId:guid}/assign", async (
                Guid instanceId,
                Guid taskExecutionId,
                AssignTaskRequest request,
                ClaimsPrincipal user,
                ICommandHandler<AssignTaskCommand, AssignTaskResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var command = request.ToCommand(instanceId, taskExecutionId, user.GetUserId(), user.IsExactlySupervisor());
                var result = await handler.Handle(command, cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.SupervisorOrAbove)
            .WithName("AssignTask")
            .Produces<AssignTaskResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status403Forbidden);
    }
}
