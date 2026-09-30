using System.Security.Claims;
using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;
using Microsoft.AspNetCore.Mvc;

namespace HotelChecklist.Api.Features.Calls.Create;

public static class CreateCallEndpoint
{
    public static void MapCreateCall(this RouteGroupBuilder group)
    {
        group.MapPost("/", async (
                [FromForm] Guid areaId,
                [FromForm] string subject,
                [FromForm] string? description,
                [FromForm] string priority,
                IFormFile? file,
                ClaimsPrincipal user,
                ICommandHandler<CreateCallCommand, CreateCallResponse> handler,
                CancellationToken cancellationToken) =>
            {
                await using var content = file?.OpenReadStream();

                var command = new CreateCallCommand(
                    areaId, subject, description, priority, user.GetUserId(),
                    content, file?.FileName, file?.ContentType, file?.Length);

                var result = await handler.Handle(command, cancellationToken);
                return result.ToHttpResult(StatusCodes.Status201Created);
            })
            .DisableAntiforgery()
            .RequireAuthorization(Policies.SupervisorOrAbove)
            .WithName("CreateCall")
            .Produces<CreateCallResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
