using System.Security.Claims;
using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.Calls.Finish;

public static class FinishCallEndpoint
{
    public static void MapFinishCall(this RouteGroupBuilder group)
    {
        group.MapPost("/{id:guid}/finish", async (
                Guid id,
                ClaimsPrincipal user,
                ICommandHandler<FinishCallCommand, FinishCallResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new FinishCallCommand(id, user.GetUserId(), user.IsSupervisorOrAbove());
                var result = await handler.Handle(command, cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.AnyRole)
            .WithName("FinishCall")
            .Produces<FinishCallResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status403Forbidden);
    }
}
