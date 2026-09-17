namespace HotelChecklist.Api.Features.ChecklistTemplates.Update;

public sealed record UpdateChecklistTemplateRequest(
    string Name,
    string? Description,
    Guid AreaId,
    string RecurrenceType,
    int EstimatedDurationMinutes,
    string ScheduledTime,
    DateOnly? RecurrenceStartDate,
    string? CustomRecurrenceMode,
    int? RecurrenceIntervalValue,
    string? RecurrenceIntervalUnit,
    List<string>? RecurrenceDaysOfWeek,
    List<ChecklistTaskRequest> Tasks);
