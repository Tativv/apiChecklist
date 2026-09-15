namespace HotelChecklist.Api.Features.Users.Update;

public sealed record UpdateUserResponse(Guid Id, string Name, string Email, string Role, bool Active);
