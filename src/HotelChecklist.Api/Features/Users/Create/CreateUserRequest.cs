namespace HotelChecklist.Api.Features.Users.Create;

public sealed record CreateUserRequest(string Name, string Email, string Password, string Role, List<Guid>? AreaIds);
