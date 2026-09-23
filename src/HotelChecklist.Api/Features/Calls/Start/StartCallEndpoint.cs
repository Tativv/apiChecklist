using System.Security.Claims;
using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.Calls.Start;

public static class StartCallEndpoint
{
    public static void MapStartCall(this RouteGroupBuilder group)
    {
        group.MapPost("/{id:guid}/start", async (
                Guid id,
                ClaimsPrincipal user,
                ICommandHandler<StartCallCommand, StartCallResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new StartCallCommand(id, user.GetUserId(), user.IsSupervisorOrAbove());
                var result = await handler.Handle(command, cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.AnyRole)
            .WithName("StartCall")
            .Produces<StartCallResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status403Forbidden);
    }
}
