namespace HotelChecklist.Api.Features.ChecklistTemplates.Create;

public sealed record CreateChecklistTemplateRequest(
    string Name,
    string? Description,
    Guid AreaId,
    string ExecutionMode,
    List<ScheduleInput> Schedules,
    List<ChecklistTaskRequest> Tasks);
