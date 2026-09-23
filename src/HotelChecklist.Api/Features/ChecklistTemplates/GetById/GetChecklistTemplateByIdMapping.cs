using HotelChecklist.Domain.Entities;

namespace HotelChecklist.Api.Features.ChecklistTemplates.GetById;

public static class GetChecklistTemplateByIdMapping
{
    public static GetChecklistTemplateByIdResponse ToResponse(this ChecklistTemplate template) => new(
        template.Id,
        template.Name,
        template.Description,
        template.AreaId,
        template.EstimatedDurationMinutes,
        template.ExecutionMode.ToString(),
        template.TemplateSchedules.OrderBy(ts => ts.Schedule.ExecutionOrder).Select(ts => ts.Schedule.ToResponseItem()).ToList(),
        template.Tasks.OrderBy(t => t.Order).Select(t => t.ToResponseItem()).ToList(),
        template.TemplateAssets.Select(ta => ta.AssetId).ToList());
}
