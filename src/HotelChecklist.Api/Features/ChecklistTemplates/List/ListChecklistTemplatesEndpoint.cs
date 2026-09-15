using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.ChecklistTemplates.List;

public static class ListChecklistTemplatesEndpoint
{
    public static void MapListChecklistTemplates(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (
                Guid? areaId,
                IQueryHandler<ListChecklistTemplatesQuery, IReadOnlyList<ListChecklistTemplatesResponseItem>> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new ListChecklistTemplatesQuery(areaId), cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.AnyRole)
            .WithName("ListChecklistTemplates")
            .Produces<IReadOnlyList<ListChecklistTemplatesResponseItem>>();
    }
}
