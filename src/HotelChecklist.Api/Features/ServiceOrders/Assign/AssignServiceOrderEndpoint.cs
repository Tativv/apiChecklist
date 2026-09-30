using System.Security.Claims;
using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.ServiceOrders.Assign;

public static class AssignServiceOrderEndpoint
{
    public static void MapAssignServiceOrder(this RouteGroupBuilder group)
    {
        group.MapPost("/{id:guid}/assign", async (
                Guid id,
                AssignServiceOrderRequest request,
                ClaimsPrincipal user,
                ICommandHandler<AssignServiceOrderCommand, AssignServiceOrderResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var command = request.ToCommand(id, user.GetUserId(), user.IsExactlySupervisor());
                var result = await handler.Handle(command, cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.SupervisorOrAbove)
            .WithName("AssignServiceOrder")
            .Produces<AssignServiceOrderResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status403Forbidden);
    }
}
