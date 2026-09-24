namespace HotelChecklist.Api.Features.ChecklistTemplates.Update;

public sealed record UpdateChecklistTemplateRequest(
    string Name,
    string? Description,
    Guid AreaId,
    string ExecutionMode,
    List<ScheduleInput> Schedules,
    List<ChecklistTaskRequest> Tasks);
