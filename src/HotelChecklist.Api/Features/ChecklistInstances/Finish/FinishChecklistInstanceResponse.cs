namespace HotelChecklist.Api.Features.ChecklistInstances.Finish;

public sealed record FinishChecklistInstanceResponse(Guid Id, string Status, DateTimeOffset? CompletedAt, long? DurationSeconds);
