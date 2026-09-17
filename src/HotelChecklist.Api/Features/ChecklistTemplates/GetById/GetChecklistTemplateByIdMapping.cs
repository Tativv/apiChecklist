using HotelChecklist.Domain.Entities;

namespace HotelChecklist.Api.Features.ChecklistTemplates.GetById;

public static class GetChecklistTemplateByIdMapping
{
    public static GetChecklistTemplateByIdResponse ToResponse(this ChecklistTemplate template) => new(
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
        template.Tasks.OrderBy(t => t.Order).Select(t => new ChecklistTaskResponseItem(t.Id, t.Name, t.Description, t.Order)).ToList(),
        template.TemplateAssets.Select(ta => ta.AssetId).ToList());
}
