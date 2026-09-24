using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.ChecklistInstances.RestartTask;

public static class RestartTaskEndpoint
{
    public static void MapRestartTask(this RouteGroupBuilder group)
    {
        group.MapPost("/{instanceId:guid}/tasks/{taskExecutionId:guid}/restart", async (
                Guid instanceId,
                Guid taskExecutionId,
                ICommandHandler<RestartTaskCommand, RestartTaskResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new RestartTaskCommand(instanceId, taskExecutionId);
                var result = await handler.Handle(command, cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.SupervisorOrAbove)
            .WithName("RestartTask")
            .Produces<RestartTaskResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
