using HotelChecklist.Domain.Entities;
using HotelChecklist.Domain.Enums;

namespace HotelChecklist.Api.Features.ChecklistTemplates.Create;

public static class CreateChecklistTemplateMapping
{
    public static CreateChecklistTemplateCommand ToCommand(this CreateChecklistTemplateRequest request, UserRole actingUserRole) =>
        new(
            request.Name,
            request.Description,
            request.AreaId,
            request.ExecutionMode,
            request.Schedules,
            request.Tasks,
            actingUserRole);

    public static CreateChecklistTemplateResponse ToResponse(this ChecklistTemplate template) => new(
        template.Id,
        template.Name,
        template.Description,
        template.AreaId,
        template.ExecutionMode.ToString(),
        template.CreatedByRole.ToString(),
        template.TemplateSchedules.OrderBy(ts => ts.Schedule.ExecutionOrder).Select(ts => ts.Schedule.ToResponseItem()).ToList(),
        template.Tasks.OrderBy(t => t.Order).Select(t => t.ToResponseItem()).ToList());
}
