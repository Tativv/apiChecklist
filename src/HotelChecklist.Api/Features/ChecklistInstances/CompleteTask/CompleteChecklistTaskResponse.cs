namespace HotelChecklist.Api.Features.ChecklistInstances.CompleteTask;

public sealed record CompleteChecklistTaskResponse(Guid Id, string Status, DateTimeOffset? ExecutedAtUtc, string? Comment);
