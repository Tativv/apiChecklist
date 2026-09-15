using HotelChecklist.Domain.Entities;

namespace HotelChecklist.Api.Features.ChecklistTemplates.Update;

public static class UpdateChecklistTemplateMapping
{
    public static UpdateChecklistTemplateCommand ToCommand(this UpdateChecklistTemplateRequest request, Guid id) =>
        new(id, request.Name, request.Description, request.AreaId, request.RecurrenceType, request.EstimatedDurationMinutes, request.Tasks);

    public static UpdateChecklistTemplateResponse ToResponse(this ChecklistTemplate template) => new(
        template.Id,
        template.Name,
        template.Description,
        template.AreaId,
        template.RecurrenceType.ToString(),
        template.EstimatedDurationMinutes,
        template.Tasks.OrderBy(t => t.Order).Select(t => new ChecklistTaskResponseItem(t.Id, t.Name, t.Order)).ToList());
}
