namespace HotelChecklist.Api.Features.ChecklistInstances.CompleteTask;

public sealed record CompleteChecklistTaskResponse(Guid Id, string Status, DateTimeOffset? CompletedAt, long? DurationSeconds, string? Comment);
