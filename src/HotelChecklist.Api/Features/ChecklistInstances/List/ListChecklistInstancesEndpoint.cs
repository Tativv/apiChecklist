using System.Security.Claims;
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
                ClaimsPrincipal user,
                IQueryHandler<ListChecklistInstancesQuery, IReadOnlyList<ListChecklistInstancesResponseItem>> handler,
                CancellationToken cancellationToken) =>
            {
                var query = new ListChecklistInstancesQuery(
                    fromDate, toDate, areaId, assetId, status, user.GetUserId(), user.IsExactlySupervisor());
                var result = await handler.Handle(query, cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.AnyRole)
            .WithName("ListChecklistInstances")
            .Produces<IReadOnlyList<ListChecklistInstancesResponseItem>>();
    }
}
