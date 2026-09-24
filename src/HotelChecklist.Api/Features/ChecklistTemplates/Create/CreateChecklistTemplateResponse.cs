namespace HotelChecklist.Api.Features.ChecklistTemplates.Create;

public sealed record CreateChecklistTemplateResponse(
    Guid Id,
    string Name,
    string? Description,
    Guid AreaId,
    int EstimatedDurationMinutes,
    string ExecutionMode,
    string CreatedByRole,
    List<ScheduleResponseItem> Schedules,
    List<ChecklistTaskResponseItem> Tasks);
