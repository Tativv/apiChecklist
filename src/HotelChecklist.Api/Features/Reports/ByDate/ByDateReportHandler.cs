using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.Reports.ByDate;

public sealed class ByDateReportHandler(AppDbContext db) : IQueryHandler<ByDateReportQuery, IReadOnlyList<ByDateReportResponseItem>>
{
    public async Task<Result<IReadOnlyList<ByDateReportResponseItem>>> Handle(ByDateReportQuery query, CancellationToken cancellationToken)
    {
        var instanceRows = await db.ChecklistInstances
            .Where(i => i.Date >= query.FromDate && i.Date <= query.ToDate)
            .Select(i => new { i.Date, i.Status, i.DurationSeconds })
            .ToListAsync(cancellationToken);

        var taskRows = await db.ChecklistTaskExecutions
            .Where(e => e.ChecklistInstance.Date >= query.FromDate && e.ChecklistInstance.Date <= query.ToDate)
            .Select(e => new { e.ChecklistInstance.Date, e.Status, e.DurationSeconds })
            .ToListAsync(cancellationToken);

        var instancesByDate = instanceRows.ToLookup(r => r.Date);
        var tasksByDate = taskRows.ToLookup(r => r.Date);
        var dates = instancesByDate.Select(g => g.Key).Union(tasksByDate.Select(g => g.Key)).OrderBy(d => d);

        var report = dates
            .Select(date =>
            {
                var instances = instancesByDate[date].ToList();
                var tasks = tasksByDate[date].ToList();

                return new ByDateReportResponseItem(
                    date,
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
            .ToList();

        return Result.Success<IReadOnlyList<ByDateReportResponseItem>>(report);
    }
}
