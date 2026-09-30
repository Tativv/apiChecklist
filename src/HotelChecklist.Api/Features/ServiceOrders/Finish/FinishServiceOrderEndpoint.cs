using System.Security.Claims;
using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.ServiceOrders.Finish;

public static class FinishServiceOrderEndpoint
{
    public static void MapFinishServiceOrder(this RouteGroupBuilder group)
    {
        group.MapPost("/{id:guid}/finish", async (
                Guid id,
                ClaimsPrincipal user,
                ICommandHandler<FinishServiceOrderCommand, FinishServiceOrderResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new FinishServiceOrderCommand(id, user.GetUserId());
                var result = await handler.Handle(command, cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.SupervisorOrAbove)
            .WithName("FinishServiceOrder")
            .Produces<FinishServiceOrderResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status403Forbidden);
    }
}
