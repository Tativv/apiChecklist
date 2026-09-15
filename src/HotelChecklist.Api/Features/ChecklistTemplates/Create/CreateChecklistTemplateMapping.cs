using HotelChecklist.Domain.Entities;

namespace HotelChecklist.Api.Features.ChecklistTemplates.Create;

public static class CreateChecklistTemplateMapping
{
    public static CreateChecklistTemplateCommand ToCommand(this CreateChecklistTemplateRequest request) =>
        new(request.Name, request.Description, request.AreaId, request.RecurrenceType, request.EstimatedDurationMinutes, request.Tasks);

    public static CreateChecklistTemplateResponse ToResponse(this ChecklistTemplate template) => new(
        template.Id,
        template.Name,
        template.Description,
        template.AreaId,
        template.RecurrenceType.ToString(),
        template.EstimatedDurationMinutes,
        template.Tasks.OrderBy(t => t.Order).Select(t => new ChecklistTaskResponseItem(t.Id, t.Name, t.Order)).ToList());
}
