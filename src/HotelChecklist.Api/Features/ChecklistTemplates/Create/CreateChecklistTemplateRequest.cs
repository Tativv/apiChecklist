namespace HotelChecklist.Api.Features.ChecklistTemplates.Create;

public sealed record CreateChecklistTemplateRequest(
    string Name,
    string? Description,
    Guid AreaId,
    int EstimatedDurationMinutes,
    List<ScheduleInput> Schedules,
    List<ChecklistTaskRequest> Tasks);
