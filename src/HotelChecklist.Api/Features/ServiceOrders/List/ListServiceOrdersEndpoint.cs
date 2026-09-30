using System.Security.Claims;
using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.ServiceOrders.List;

public static class ListServiceOrdersEndpoint
{
    public static void MapListServiceOrders(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (
                Guid? areaId,
                Guid? assetId,
                string? status,
                string? priority,
                Guid? assignedUserId,
                ClaimsPrincipal user,
                IQueryHandler<ListServiceOrdersQuery, IReadOnlyList<ListServiceOrdersResponseItem>> handler,
                CancellationToken cancellationToken) =>
            {
                var query = new ListServiceOrdersQuery(
                    areaId, assetId, status, priority, assignedUserId, user.GetUserId(),
                    RestrictToSupervisedAreas: user.IsExactlySupervisor(),
                    RestrictToOwnOrUnassignedInAreas: user.IsExactlyColaborador());

                var result = await handler.Handle(query, cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.AnyRole)
            .WithName("ListServiceOrders")
            .Produces<IReadOnlyList<ListServiceOrdersResponseItem>>();
    }
}
