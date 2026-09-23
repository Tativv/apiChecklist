using System.Security.Claims;
using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;
using HotelChecklist.Api.Common.Validation;

namespace HotelChecklist.Api.Features.ChecklistInstances.CompleteTask;

public static class CompleteChecklistTaskEndpoint
{
    public static void MapCompleteChecklistTask(this RouteGroupBuilder group)
    {
        group.MapPost("/{instanceId:guid}/tasks/{taskExecutionId:guid}/complete", async (
                Guid instanceId,
                Guid taskExecutionId,
                CompleteChecklistTaskRequest request,
                ClaimsPrincipal user,
                ICommandHandler<CompleteChecklistTaskCommand, CompleteChecklistTaskResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(
                    request.ToCommand(instanceId, taskExecutionId, user.GetUserId(), user.IsSupervisorOrAbove()), cancellationToken);
                return result.ToHttpResult();
            })
            .AddEndpointFilter<ValidationFilter<CompleteChecklistTaskRequest>>()
            .RequireAuthorization(Policies.AnyRole)
            .WithName("CompleteChecklistTask")
            .Produces<CompleteChecklistTaskResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status403Forbidden);
    }
}
