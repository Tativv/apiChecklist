using System.Security.Claims;
using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;
using HotelChecklist.Api.Common.Validation;

namespace HotelChecklist.Api.Features.Calls.ConvertToServiceOrder;

public static class ConvertCallToServiceOrderEndpoint
{
    public static void MapConvertCallToServiceOrder(this RouteGroupBuilder group)
    {
        group.MapPost("/{id:guid}/convert-to-service-order", async (
                Guid id,
                ConvertCallToServiceOrderRequest request,
                ClaimsPrincipal user,
                ICommandHandler<ConvertCallToServiceOrderCommand, ConvertCallToServiceOrderResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(request.ToCommand(id, user.GetUserId()), cancellationToken);
                return result.ToHttpResult(StatusCodes.Status201Created);
            })
            .AddEndpointFilter<ValidationFilter<ConvertCallToServiceOrderRequest>>()
            .RequireAuthorization(Policies.SupervisorOrAbove)
            .WithName("ConvertCallToServiceOrder")
            .Produces<ConvertCallToServiceOrderResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
