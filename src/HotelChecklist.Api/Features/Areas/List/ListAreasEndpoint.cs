using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.Areas.List;

public static class ListAreasEndpoint
{
    public static void MapListAreas(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (
                IQueryHandler<ListAreasQuery, IReadOnlyList<ListAreasResponseItem>> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new ListAreasQuery(), cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.AnyRole)
            .WithName("ListAreas")
            .Produces<IReadOnlyList<ListAreasResponseItem>>();
    }
}
