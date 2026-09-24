using System.Security.Claims;
using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;
using Microsoft.AspNetCore.Mvc;

namespace HotelChecklist.Api.Features.ChecklistInstances.TaskComments;

public static class AddTaskCommentEndpoint
{
    public static void MapAddTaskComment(this RouteGroupBuilder group)
    {
        group.MapPost("/{instanceId:guid}/tasks/{taskExecutionId:guid}/comments", async (
                Guid instanceId,
                Guid taskExecutionId,
                [FromForm] string? text,
                IFormFile? file,
                ClaimsPrincipal user,
                ICommandHandler<AddTaskCommentCommand, TaskCommentResponseItem> handler,
                CancellationToken cancellationToken) =>
            {
                await using var content = file?.OpenReadStream();

                var command = new AddTaskCommentCommand(
                    instanceId, taskExecutionId, text, user.GetUserId(),
                    content, file?.FileName, file?.ContentType, file?.Length);

                var result = await handler.Handle(command, cancellationToken);
                return result.ToHttpResult(StatusCodes.Status201Created);
            })
            .DisableAntiforgery()
            .RequireAuthorization(Policies.AnyRole)
            .WithName("AddTaskComment")
            .Produces<TaskCommentResponseItem>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
