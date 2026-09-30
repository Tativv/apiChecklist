using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;
using HotelChecklist.Api.Common.Validation;

namespace HotelChecklist.Api.Features.ServiceOrders.Update;

public static class UpdateServiceOrderEndpoint
{
    public static void MapUpdateServiceOrder(this RouteGroupBuilder group)
    {
        group.MapPut("/{id:guid}", async (
                Guid id,
                UpdateServiceOrderRequest request,
                ICommandHandler<UpdateServiceOrderCommand, UpdateServiceOrderResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(request.ToCommand(id), cancellationToken);
                return result.ToHttpResult();
            })
            .AddEndpointFilter<ValidationFilter<UpdateServiceOrderRequest>>()
            .RequireAuthorization(Policies.SupervisorOrAbove)
            .WithName("UpdateServiceOrder")
            .Produces<UpdateServiceOrderResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
