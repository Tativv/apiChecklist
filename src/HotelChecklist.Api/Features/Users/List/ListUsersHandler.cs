using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.Users.List;

public sealed class ListUsersHandler(AppDbContext db) : IQueryHandler<ListUsersQuery, IReadOnlyList<ListUsersResponseItem>>
{
    public async Task<Result<IReadOnlyList<ListUsersResponseItem>>> Handle(ListUsersQuery query, CancellationToken cancellationToken)
    {
        var usersQuery = db.Users.AsQueryable();

        if (query.Active is not null)
            usersQuery = usersQuery.Where(u => u.Active == query.Active);

        if (!string.IsNullOrWhiteSpace(query.Role) && Enum.TryParse<UserRole>(query.Role, ignoreCase: true, out var role))
            usersQuery = usersQuery.Where(u => u.Role == role);

        var users = await usersQuery
            .OrderByDescending(u => u.CreatedAtUtc)
            .Select(u => new ListUsersResponseItem(
                u.Id, u.Name, u.Email, u.Role.ToString(), u.Active, u.UserAreas.Select(ua => ua.AreaId).ToList()))
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<ListUsersResponseItem>>(users);
    }
}
