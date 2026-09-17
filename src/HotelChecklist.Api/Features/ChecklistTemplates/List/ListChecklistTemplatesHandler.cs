using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ChecklistTemplates.List;

public sealed class ListChecklistTemplatesHandler(AppDbContext db) : IQueryHandler<ListChecklistTemplatesQuery, IReadOnlyList<ListChecklistTemplatesResponseItem>>
{
    public async Task<Result<IReadOnlyList<ListChecklistTemplatesResponseItem>>> Handle(ListChecklistTemplatesQuery query, CancellationToken cancellationToken)
    {
        var templatesQuery = db.ChecklistTemplates.AsQueryable();

        if (query.AreaId is not null)
            templatesQuery = templatesQuery.Where(t => t.AreaId == query.AreaId);

        var rows = await templatesQuery
            .OrderBy(t => t.Name)
            .Select(t => new
            {
                t.Id,
                t.Name,
                t.AreaId,
                t.RecurrenceType,
                t.EstimatedDurationMinutes,
                t.ScheduledTime,
                TaskCount = t.Tasks.Count,
                AssetCount = t.TemplateAssets.Count
            })
            .ToListAsync(cancellationToken);

        var templates = rows
            .Select(t => new ListChecklistTemplatesResponseItem(
                t.Id,
                t.Name,
                t.AreaId,
                t.RecurrenceType.ToString(),
                t.EstimatedDurationMinutes,
                ChecklistTemplateSchedulingMapping.FormatScheduledTime(t.ScheduledTime),
                t.TaskCount,
                t.AssetCount))
            .ToList();

        return Result.Success<IReadOnlyList<ListChecklistTemplatesResponseItem>>(templates);
    }
}
