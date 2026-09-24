using System.Security.Claims;
using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;
using Microsoft.AspNetCore.Mvc;

namespace HotelChecklist.Api.Features.Calls.CallComments;

public static class AddCallCommentEndpoint
{
    public static void MapAddCallComment(this RouteGroupBuilder group)
    {
        group.MapPost("/{callId:guid}/comments", async (
                Guid callId,
                [FromForm] string? text,
                IFormFile? file,
                ClaimsPrincipal user,
                ICommandHandler<AddCallCommentCommand, CallCommentResponseItem> handler,
                CancellationToken cancellationToken) =>
            {
                await using var content = file?.OpenReadStream();

                var command = new AddCallCommentCommand(
                    callId, text, user.GetUserId(),
                    content, file?.FileName, file?.ContentType, file?.Length);

                var result = await handler.Handle(command, cancellationToken);
                return result.ToHttpResult(StatusCodes.Status201Created);
            })
            .DisableAntiforgery()
            .RequireAuthorization(Policies.AnyRole)
            .WithName("AddCallComment")
            .Produces<CallCommentResponseItem>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
