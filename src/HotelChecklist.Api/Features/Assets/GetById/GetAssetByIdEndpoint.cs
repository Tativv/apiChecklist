using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.Assets.GetById;

public static class GetAssetByIdEndpoint
{
    public static void MapGetAssetById(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", async (
                Guid id,
                IQueryHandler<GetAssetByIdQuery, GetAssetByIdResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new GetAssetByIdQuery(id), cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.AnyRole)
            .WithName("GetAssetById")
            .Produces<GetAssetByIdResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
