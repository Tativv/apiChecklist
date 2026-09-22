namespace HotelChecklist.Api.Features.Users.Create;

public sealed record CreateUserResponse(Guid Id, string Name, string Email, string Role, bool Active, List<Guid> AreaIds);
