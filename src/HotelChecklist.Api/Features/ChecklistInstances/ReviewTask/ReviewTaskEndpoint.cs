using System.Security.Claims;
using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.ChecklistInstances.ReviewTask;

public static class ReviewTaskEndpoint
{
    public static void MapReviewTask(this RouteGroupBuilder group)
    {
        group.MapPost("/{instanceId:guid}/tasks/{taskExecutionId:guid}/review", async (
                Guid instanceId,
                Guid taskExecutionId,
                ClaimsPrincipal user,
                ICommandHandler<ReviewTaskCommand, ReviewTaskResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new ReviewTaskCommand(instanceId, taskExecutionId, user.GetUserId());
                var result = await handler.Handle(command, cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.SupervisorOrAbove)
            .WithName("ReviewTask")
            .Produces<ReviewTaskResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
