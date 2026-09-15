using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.ChecklistInstances.List;

public static class ListChecklistInstancesEndpoint
{
    public static void MapListChecklistInstances(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (
                DateOnly? fromDate,
                DateOnly? toDate,
                Guid? areaId,
                Guid? assetId,
                string? status,
                Guid? assignedUserId,
                IQueryHandler<ListChecklistInstancesQuery, IReadOnlyList<ListChecklistInstancesResponseItem>> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(
                    new ListChecklistInstancesQuery(fromDate, toDate, areaId, assetId, status, assignedUserId), cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.AnyRole)
            .WithName("ListChecklistInstances")
            .Produces<IReadOnlyList<ListChecklistInstancesResponseItem>>();
    }
}
