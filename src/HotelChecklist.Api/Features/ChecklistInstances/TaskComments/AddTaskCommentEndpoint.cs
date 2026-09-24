using System.Security.Claims;
using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;
using HotelChecklist.Api.Common.Validation;

namespace HotelChecklist.Api.Features.ChecklistInstances.TaskComments;

public static class AddTaskCommentEndpoint
{
    public static void MapAddTaskComment(this RouteGroupBuilder group)
    {
        group.MapPost("/{instanceId:guid}/tasks/{taskExecutionId:guid}/comments", async (
                Guid instanceId,
                Guid taskExecutionId,
                AddTaskCommentRequest request,
                ClaimsPrincipal user,
                ICommandHandler<AddTaskCommentCommand, TaskCommentResponseItem> handler,
                CancellationToken cancellationToken) =>
            {
                var command = request.ToCommand(instanceId, taskExecutionId, user.GetUserId());
                var result = await handler.Handle(command, cancellationToken);
                return result.ToHttpResult(StatusCodes.Status201Created);
            })
            .AddEndpointFilter<ValidationFilter<AddTaskCommentRequest>>()
            .RequireAuthorization(Policies.AnyRole)
            .WithName("AddTaskComment")
            .Produces<TaskCommentResponseItem>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
