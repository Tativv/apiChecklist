namespace HotelChecklist.Api.Features.Users.GetById;

public sealed record GetUserByIdResponse(Guid Id, string Name, string Email, string Role, bool Active);
