using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.Assets.List;

public static class ListAssetsEndpoint
{
    public static void MapListAssets(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (
                Guid? areaId,
                bool? active,
                IQueryHandler<ListAssetsQuery, IReadOnlyList<ListAssetsResponseItem>> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new ListAssetsQuery(areaId, active), cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.AnyRole)
            .WithName("ListAssets")
            .Produces<IReadOnlyList<ListAssetsResponseItem>>();
    }
}
