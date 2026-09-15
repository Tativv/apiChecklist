namespace HotelChecklist.Api.Features.ChecklistInstances.Start;

public sealed record StartChecklistInstanceResponse(Guid Id, string Status, DateTimeOffset? StartedAt, Guid? AssignedUserId);
