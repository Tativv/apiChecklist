namespace HotelChecklist.Api.Features.ChecklistInstances.CompleteTask;

public sealed record CompleteChecklistTaskResponse(Guid Id, bool Completed, DateTimeOffset? CompletedAt, string? Comment);
