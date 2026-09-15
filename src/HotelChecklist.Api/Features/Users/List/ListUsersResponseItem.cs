namespace HotelChecklist.Api.Features.Users.List;

public sealed record ListUsersResponseItem(Guid Id, string Name, string Email, string Role, bool Active);
