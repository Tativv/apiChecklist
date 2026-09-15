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
        var rows = await db.ChecklistInstances
            .Where(i => i.Date >= query.FromDate && i.Date <= query.ToDate)
            .Select(i => new { i.Date, i.Status, i.DurationSeconds })
            .ToListAsync(cancellationToken);

        var report = rows
            .GroupBy(r => r.Date)
            .Select(g => new ByDateReportResponseItem(
                g.Key,
                g.Count(),
                g.Count(r => r.Status == ChecklistStatus.Pending),
                g.Count(r => r.Status == ChecklistStatus.InProgress),
                g.Count(r => r.Status == ChecklistStatus.Completed),
                g.Count(r => r.Status == ChecklistStatus.Approved),
                g.Any(r => r.DurationSeconds != null) ? g.Where(r => r.DurationSeconds != null).Average(r => r.DurationSeconds!.Value) : null))
            .OrderBy(item => item.Date)
            .ToList();

        return Result.Success<IReadOnlyList<ByDateReportResponseItem>>(report);
    }
}
