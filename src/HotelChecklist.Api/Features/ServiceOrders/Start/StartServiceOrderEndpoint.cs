using System.Security.Claims;
using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.ServiceOrders.Start;

public static class StartServiceOrderEndpoint
{
    public static void MapStartServiceOrder(this RouteGroupBuilder group)
    {
        group.MapPost("/{id:guid}/start", async (
                Guid id,
                ClaimsPrincipal user,
                ICommandHandler<StartServiceOrderCommand, StartServiceOrderResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new StartServiceOrderCommand(id, user.GetUserId());
                var result = await handler.Handle(command, cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.SupervisorOrAbove)
            .WithName("StartServiceOrder")
            .Produces<StartServiceOrderResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status403Forbidden);
    }
}
