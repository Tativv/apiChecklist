namespace HotelChecklist.Api.Features.ChecklistInstances.GenerateScheduled;

public sealed record GenerateScheduledChecklistsResponse(DateOnly Date, int Created, int Skipped);
