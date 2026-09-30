using System.Security.Claims;
using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;
using HotelChecklist.Api.Common.Validation;

namespace HotelChecklist.Api.Features.ServiceOrders.Create;

public static class CreateServiceOrderEndpoint
{
    public static void MapCreateServiceOrder(this RouteGroupBuilder group)
    {
        group.MapPost("/", async (
                CreateServiceOrderRequest request,
                ClaimsPrincipal user,
                ICommandHandler<CreateServiceOrderCommand, CreateServiceOrderResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(request.ToCommand(user.GetUserId()), cancellationToken);
                return result.ToHttpResult(StatusCodes.Status201Created);
            })
            .AddEndpointFilter<ValidationFilter<CreateServiceOrderRequest>>()
            .RequireAuthorization(Policies.SupervisorOrAbove)
            .WithName("CreateServiceOrder")
            .Produces<CreateServiceOrderResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
