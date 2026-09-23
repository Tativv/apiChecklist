namespace HotelChecklist.Api.Features.Calls.Start;

public sealed record StartCallResponse(Guid Id, string Status, DateTimeOffset? StartedAt);
