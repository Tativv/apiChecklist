using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.Users.List;

public static class ListUsersEndpoint
{
    public static void MapListUsers(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (
                bool? active,
                string? role,
                IQueryHandler<ListUsersQuery, IReadOnlyList<ListUsersResponseItem>> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new ListUsersQuery(active, role), cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.SupervisorOrAbove)
            .WithName("ListUsers")
            .Produces<IReadOnlyList<ListUsersResponseItem>>();
    }
}
