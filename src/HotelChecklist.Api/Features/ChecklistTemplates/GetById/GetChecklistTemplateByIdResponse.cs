namespace HotelChecklist.Api.Features.ChecklistTemplates.GetById;

public sealed record GetChecklistTemplateByIdResponse(
    Guid Id,
    string Name,
    string? Description,
    Guid AreaId,
    int EstimatedDurationMinutes,
    List<ScheduleResponseItem> Schedules,
    List<ChecklistTaskResponseItem> Tasks,
    List<Guid> AssetIds);
