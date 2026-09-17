using HotelChecklist.Domain.Entities;

namespace HotelChecklist.Api.Features.ChecklistTemplates.Update;

public static class UpdateChecklistTemplateMapping
{
    public static UpdateChecklistTemplateCommand ToCommand(this UpdateChecklistTemplateRequest request, Guid id) =>
        new(
            id,
            request.Name,
            request.Description,
            request.AreaId,
            request.RecurrenceType,
            request.EstimatedDurationMinutes,
            request.ScheduledTime,
            request.RecurrenceStartDate,
            request.CustomRecurrenceMode,
            request.RecurrenceIntervalValue,
            request.RecurrenceIntervalUnit,
            request.RecurrenceDaysOfWeek,
            request.Tasks);

    public static UpdateChecklistTemplateResponse ToResponse(this ChecklistTemplate template) => new(
        template.Id,
        template.Name,
        template.Description,
        template.AreaId,
        template.RecurrenceType.ToString(),
        template.EstimatedDurationMinutes,
        ChecklistTemplateSchedulingMapping.FormatScheduledTime(template.ScheduledTime),
        template.RecurrenceStartDate,
        template.CustomRecurrenceMode?.ToString(),
        template.RecurrenceIntervalValue,
        template.RecurrenceIntervalUnit?.ToString(),
        ChecklistTemplateSchedulingMapping.ToDaysOfWeekList(template.RecurrenceDaysOfWeekMask),
        template.Tasks.OrderBy(t => t.Order).Select(t => new ChecklistTaskResponseItem(t.Id, t.Name, t.Description, t.Order)).ToList());
}
