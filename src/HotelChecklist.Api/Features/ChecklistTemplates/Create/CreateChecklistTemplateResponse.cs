namespace HotelChecklist.Api.Features.ChecklistTemplates.Create;

public sealed record CreateChecklistTemplateResponse(
    Guid Id,
    string Name,
    string? Description,
    Guid AreaId,
    string ExecutionMode,
    string CreatedByRole,
    List<ScheduleResponseItem> Schedules,
    List<ChecklistTaskResponseItem> Tasks);
