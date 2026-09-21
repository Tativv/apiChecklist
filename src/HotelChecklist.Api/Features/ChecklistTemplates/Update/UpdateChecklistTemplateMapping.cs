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
            request.EstimatedDurationMinutes,
            request.Schedules,
            request.Tasks);

    public static UpdateChecklistTemplateResponse ToResponse(this ChecklistTemplate template) => new(
        template.Id,
        template.Name,
        template.Description,
        template.AreaId,
        template.EstimatedDurationMinutes,
        template.TemplateSchedules.OrderBy(ts => ts.Schedule.ExecutionOrder).Select(ts => ts.Schedule.ToResponseItem()).ToList(),
        template.Tasks.OrderBy(t => t.Order).Select(t => t.ToResponseItem()).ToList());
}
