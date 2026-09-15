namespace HotelChecklist.Api.Features.ChecklistInstances.GenerateDaily;

public sealed record GenerateDailyChecklistsResponse(DateOnly Date, int Created, int Skipped);
