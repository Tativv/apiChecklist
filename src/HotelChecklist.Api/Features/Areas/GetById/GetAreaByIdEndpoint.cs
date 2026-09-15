using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.Areas.GetById;

public static class GetAreaByIdEndpoint
{
    public static void MapGetAreaById(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", async (
                Guid id,
                IQueryHandler<GetAreaByIdQuery, GetAreaByIdResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new GetAreaByIdQuery(id), cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.AnyRole)
            .WithName("GetAreaById")
            .Produces<GetAreaByIdResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
