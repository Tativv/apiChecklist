using HotelChecklist.Domain.Entities;

namespace HotelChecklist.Api.Features.ChecklistTemplates.Create;

public static class CreateChecklistTemplateMapping
{
    public static CreateChecklistTemplateCommand ToCommand(this CreateChecklistTemplateRequest request) =>
        new(
            request.Name,
            request.Description,
            request.AreaId,
            request.EstimatedDurationMinutes,
            request.ExecutionMode,
            request.Schedules,
            request.Tasks);

    public static CreateChecklistTemplateResponse ToResponse(this ChecklistTemplate template) => new(
        template.Id,
        template.Name,
        template.Description,
        template.AreaId,
        template.EstimatedDurationMinutes,
        template.ExecutionMode.ToString(),
        template.TemplateSchedules.OrderBy(ts => ts.Schedule.ExecutionOrder).Select(ts => ts.Schedule.ToResponseItem()).ToList(),
        template.Tasks.OrderBy(t => t.Order).Select(t => t.ToResponseItem()).ToList());
}
