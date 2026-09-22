namespace HotelChecklist.Api.Features.Users.Update;

public sealed record UpdateUserRequest(string Name, string Role, List<Guid>? AreaIds);
