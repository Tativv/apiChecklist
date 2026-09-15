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
        var rows = await db.ChecklistInstances
            .Where(i => i.Date >= query.FromDate && i.Date <= query.ToDate)
            .Select(i => new { i.Asset.AreaId, AreaName = i.Asset.Area.Name, i.Status, i.DurationSeconds })
            .ToListAsync(cancellationToken);

        var report = rows
            .GroupBy(r => new { r.AreaId, r.AreaName })
            .Select(g => new ByAreaReportResponseItem(
                g.Key.AreaId,
                g.Key.AreaName,
                g.Count(),
                g.Count(r => r.Status == ChecklistStatus.Pending),
                g.Count(r => r.Status == ChecklistStatus.InProgress),
                g.Count(r => r.Status == ChecklistStatus.Completed),
                g.Count(r => r.Status == ChecklistStatus.Approved),
                g.Any(r => r.DurationSeconds != null) ? g.Where(r => r.DurationSeconds != null).Average(r => r.DurationSeconds!.Value) : null))
            .OrderBy(item => item.AreaName)
            .ToList();

        return Result.Success<IReadOnlyList<ByAreaReportResponseItem>>(report);
    }
}
