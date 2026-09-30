namespace HotelChecklist.Api.Features.Calls.Update;

public sealed record UpdateCallRequest(
    Guid AreaId,
    string Subject,
    string? Description,
    string Priority);
