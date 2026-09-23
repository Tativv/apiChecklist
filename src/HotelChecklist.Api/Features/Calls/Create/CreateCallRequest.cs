namespace HotelChecklist.Api.Features.Calls.Create;

public sealed record CreateCallRequest(
    Guid AreaId,
    string Subject,
    string? Description,
    string Priority);
