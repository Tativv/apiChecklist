namespace HotelChecklist.Api.Features.Auth.Login;

public sealed record LoginResponse(
    string Token,
    DateTimeOffset ExpiresAtUtc,
    Guid UserId,
    string Name,
    string Email,
    string Role);
