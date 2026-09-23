namespace HotelChecklist.Api.Features.Calls.Finish;

public sealed record FinishCallResponse(Guid Id, string Status, DateTimeOffset? CompletedAt, long? DurationSeconds);
