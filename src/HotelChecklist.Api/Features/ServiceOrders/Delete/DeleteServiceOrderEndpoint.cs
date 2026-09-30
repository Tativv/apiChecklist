using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.ServiceOrders.Delete;

public static class DeleteServiceOrderEndpoint
{
    public static void MapDeleteServiceOrder(this RouteGroupBuilder group)
    {
        group.MapDelete("/{id:guid}", async (
                Guid id,
                ICommandHandler<DeleteServiceOrderCommand, Unit> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new DeleteServiceOrderCommand(id), cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.ManagerOrAbove)
            .WithName("DeleteServiceOrder")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
