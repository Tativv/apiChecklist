namespace HotelChecklist.Api.Features.ChecklistInstances.StartTask;

public sealed record StartTaskResponse(Guid Id, string Status, DateTimeOffset? StartedAt);
