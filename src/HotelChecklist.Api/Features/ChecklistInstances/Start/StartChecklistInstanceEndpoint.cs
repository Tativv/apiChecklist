using System.Security.Claims;
using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.ChecklistInstances.Start;

public static class StartChecklistInstanceEndpoint
{
    public static void MapStartChecklistInstance(this RouteGroupBuilder group)
    {
        group.MapPost("/{id:guid}/start", async (
                Guid id,
                ClaimsPrincipal user,
                ICommandHandler<StartChecklistInstanceCommand, StartChecklistInstanceResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new StartChecklistInstanceCommand(id, user.GetUserId(), user.IsManagerOrAbove());
                var result = await handler.Handle(command, cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.AnyRole)
            .WithName("StartChecklistInstance")
            .Produces<StartChecklistInstanceResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status403Forbidden);
    }
}
