using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.ServiceOrders.GetById;

public static class GetServiceOrderByIdEndpoint
{
    public static void MapGetServiceOrderById(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", async (
                Guid id,
                IQueryHandler<GetServiceOrderByIdQuery, GetServiceOrderByIdResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new GetServiceOrderByIdQuery(id), cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.AnyRole)
            .WithName("GetServiceOrderById")
            .Produces<GetServiceOrderByIdResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
