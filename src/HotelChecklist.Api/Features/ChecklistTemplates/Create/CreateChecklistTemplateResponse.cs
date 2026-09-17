namespace HotelChecklist.Api.Features.ChecklistTemplates.Create;

public sealed record CreateChecklistTemplateResponse(
    Guid Id,
    string Name,
    string? Description,
    Guid AreaId,
    string RecurrenceType,
    int EstimatedDurationMinutes,
    string ScheduledTime,
    DateOnly RecurrenceStartDate,
    string? CustomRecurrenceMode,
    int? RecurrenceIntervalValue,
    string? RecurrenceIntervalUnit,
    List<string> RecurrenceDaysOfWeek,
    List<ChecklistTaskResponseItem> Tasks);
