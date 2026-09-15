using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.Users.List;

public sealed record ListUsersQuery(bool? Active, string? Role) : IQuery<IReadOnlyList<ListUsersResponseItem>>;
