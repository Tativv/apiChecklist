namespace HotelChecklist.Api.Features.ChecklistTemplates.List;

public sealed record ListChecklistTemplatesResponseItem(
    Guid Id,
    string Name,
    Guid AreaId,
    int ScheduleCount,
    int TaskCount,
    int AssetCount,
    string CreatedByRole);
