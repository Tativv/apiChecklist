using HotelChecklist.Domain.Entities;
using HotelChecklist.Domain.Enums;

namespace HotelChecklist.Api.Features.ChecklistTemplates.Update;

public static class UpdateChecklistTemplateMapping
{
    public static UpdateChecklistTemplateCommand ToCommand(this UpdateChecklistTemplateRequest request, Guid id, UserRole actingUserRole) =>
        new(
            id,
            request.Name,
            request.Description,
            request.AreaId,
            request.ExecutionMode,
            request.Schedules,
            request.Tasks,
            actingUserRole);

    public static UpdateChecklistTemplateResponse ToResponse(this ChecklistTemplate template, bool versionedAsNewTemplate) => new(
        template.Id,
        template.Name,
        template.Description,
        template.AreaId,
        template.ExecutionMode.ToString(),
        template.CreatedByRole.ToString(),
        template.TemplateSchedules.OrderBy(ts => ts.Schedule.ExecutionOrder).Select(ts => ts.Schedule.ToResponseItem()).ToList(),
        template.Tasks.OrderBy(t => t.Order).Select(t => t.ToResponseItem()).ToList(),
        versionedAsNewTemplate);
}
