using System.Security.Claims;
using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.Calls.List;

public static class ListCallsEndpoint
{
    public static void MapListCalls(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (
                Guid? areaId,
                string? status,
                string? priority,
                Guid? assignedUserId,
                ClaimsPrincipal user,
                IQueryHandler<ListCallsQuery, IReadOnlyList<ListCallsResponseItem>> handler,
                CancellationToken cancellationToken) =>
            {
                var query = new ListCallsQuery(
                    areaId, status, priority, assignedUserId, user.GetUserId(),
                    RestrictToSupervisedAreas: user.IsExactlySupervisor(),
                    RestrictToOwnOrUnassignedInAreas: user.IsExactlyColaborador());

                var result = await handler.Handle(query, cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.AnyRole)
            .WithName("ListCalls")
            .Produces<IReadOnlyList<ListCallsResponseItem>>();
    }
}
