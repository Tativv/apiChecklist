using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ChecklistTemplates.List;

public sealed class ListChecklistTemplatesHandler(AppDbContext db) : IQueryHandler<ListChecklistTemplatesQuery, IReadOnlyList<ListChecklistTemplatesResponseItem>>
{
    public async Task<Result<IReadOnlyList<ListChecklistTemplatesResponseItem>>> Handle(ListChecklistTemplatesQuery query, CancellationToken cancellationToken)
    {
        var templatesQuery = db.ChecklistTemplates.Where(t => !t.IsSnapshot);

        if (query.AreaId is not null)
            templatesQuery = templatesQuery.Where(t => t.AreaId == query.AreaId);

        var templates = await templatesQuery
            .OrderBy(t => t.Name)
            .Select(t => new ListChecklistTemplatesResponseItem(
                t.Id,
                t.Name,
                t.AreaId,
                t.EstimatedDurationMinutes,
                t.TemplateSchedules.Count,
                t.Tasks.Count,
                t.TemplateAssets.Count))
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<ListChecklistTemplatesResponseItem>>(templates);
    }
}
