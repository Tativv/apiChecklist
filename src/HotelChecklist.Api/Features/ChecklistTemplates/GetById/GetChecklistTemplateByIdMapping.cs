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
        template.Tasks.OrderBy(t => t.Order).Select(t => new ChecklistTaskResponseItem(t.Id, t.Name, t.Order)).ToList());
}
