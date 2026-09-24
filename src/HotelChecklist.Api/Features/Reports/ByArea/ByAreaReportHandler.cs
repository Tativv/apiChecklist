using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.Reports.ByArea;

public sealed class ByAreaReportHandler(AppDbContext db) : IQueryHandler<ByAreaReportQuery, IReadOnlyList<ByAreaReportResponseItem>>
{
    public async Task<Result<IReadOnlyList<ByAreaReportResponseItem>>> Handle(ByAreaReportQuery query, CancellationToken cancellationToken)
    {
        var instanceRows = await db.ChecklistInstances
            .Where(i => i.Date >= query.FromDate && i.Date <= query.ToDate)
            .Select(i => new { i.Asset.AreaId, AreaName = i.Asset.Area.Name, i.Status, i.DurationSeconds })
            .ToListAsync(cancellationToken);

        var taskRows = await db.ChecklistTaskExecutions
            .Where(e => e.ChecklistInstance.Date >= query.FromDate && e.ChecklistInstance.Date <= query.ToDate)
            .Select(e => new { e.ChecklistInstance.Asset.AreaId, AreaName = e.ChecklistInstance.Asset.Area.Name, e.Status, e.DurationSeconds })
            .ToListAsync(cancellationToken);

        var instancesByArea = instanceRows.ToLookup(r => (r.AreaId, r.AreaName));
        var tasksByArea = taskRows.ToLookup(r => (r.AreaId, r.AreaName));
        var areas = instancesByArea.Select(g => g.Key).Union(tasksByArea.Select(g => g.Key));

        var report = areas
            .Select(area =>
            {
                var instances = instancesByArea[area].ToList();
                var tasks = tasksByArea[area].ToList();

                return new ByAreaReportResponseItem(
                    area.AreaId,
                    area.AreaName,
                    instances.Count,
                    instances.Count(r => r.Status == ChecklistStatus.Pending),
                    instances.Count(r => r.Status == ChecklistStatus.InProgress),
                    instances.Count(r => r.Status == ChecklistStatus.Completed),
                    instances.Any(r => r.DurationSeconds != null)
                        ? instances.Where(r => r.DurationSeconds != null).Average(r => r.DurationSeconds!.Value)
                        : null,
                    tasks.Count,
                    tasks.Count(r => r.Status == TaskExecutionStatus.Pending),
                    tasks.Count(r => r.Status == TaskExecutionStatus.InProgress),
                    tasks.Count(r => r.Status == TaskExecutionStatus.Completed),
                    tasks.Count(r => r.Status == TaskExecutionStatus.Reviewed),
                    tasks.Any(r => r.DurationSeconds != null)
                        ? tasks.Where(r => r.DurationSeconds != null).Average(r => r.DurationSeconds!.Value)
                        : null);
            })
            .OrderBy(item => item.AreaName)
            .ToList();

        return Result.Success<IReadOnlyList<ByAreaReportResponseItem>>(report);
    }
}
