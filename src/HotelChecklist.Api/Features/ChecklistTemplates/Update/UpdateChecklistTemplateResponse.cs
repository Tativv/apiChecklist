namespace HotelChecklist.Api.Features.ChecklistTemplates.Update;

public sealed record UpdateChecklistTemplateResponse(
    Guid Id,
    string Name,
    string? Description,
    Guid AreaId,
    string ExecutionMode,
    string CreatedByRole,
    List<ScheduleResponseItem> Schedules,
    List<ChecklistTaskResponseItem> Tasks,
    bool VersionedAsNewTemplate);
