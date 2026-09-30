using System.Security.Claims;
using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;
using Microsoft.AspNetCore.Mvc;

namespace HotelChecklist.Api.Features.ServiceOrders.ServiceOrderComments;

public static class AddServiceOrderCommentEndpoint
{
    public static void MapAddServiceOrderComment(this RouteGroupBuilder group)
    {
        group.MapPost("/{serviceOrderId:guid}/comments", async (
                Guid serviceOrderId,
                [FromForm] string? text,
                IFormFile? file,
                ClaimsPrincipal user,
                ICommandHandler<AddServiceOrderCommentCommand, ServiceOrderCommentResponseItem> handler,
                CancellationToken cancellationToken) =>
            {
                await using var content = file?.OpenReadStream();

                var command = new AddServiceOrderCommentCommand(
                    serviceOrderId, text, user.GetUserId(),
                    content, file?.FileName, file?.ContentType, file?.Length);

                var result = await handler.Handle(command, cancellationToken);
                return result.ToHttpResult(StatusCodes.Status201Created);
            })
            .DisableAntiforgery()
            .RequireAuthorization(Policies.AnyRole)
            .WithName("AddServiceOrderComment")
            .Produces<ServiceOrderCommentResponseItem>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
