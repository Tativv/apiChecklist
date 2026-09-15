namespace HotelChecklist.Api.Features.ChecklistTemplates.GetById;

public sealed record GetChecklistTemplateByIdResponse(
    Guid Id,
    string Name,
    string? Description,
    Guid AreaId,
    string RecurrenceType,
    int EstimatedDurationMinutes,
    List<ChecklistTaskResponseItem> Tasks);
