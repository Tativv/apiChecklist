namespace HotelChecklist.Api.Features.ChecklistTemplates.GetById;

public sealed record GetChecklistTemplateByIdResponse(
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
    List<ChecklistTaskResponseItem> Tasks,
    List<Guid> AssetIds);
