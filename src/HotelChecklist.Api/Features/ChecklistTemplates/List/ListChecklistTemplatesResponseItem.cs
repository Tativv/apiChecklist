namespace HotelChecklist.Api.Features.ChecklistTemplates.List;

public sealed record ListChecklistTemplatesResponseItem(
    Guid Id,
    string Name,
    Guid AreaId,
    string RecurrenceType,
    int EstimatedDurationMinutes,
    int TaskCount);
