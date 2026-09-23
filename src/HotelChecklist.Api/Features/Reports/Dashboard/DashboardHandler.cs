using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.Reports.Dashboard;

public sealed class DashboardHandler(AppDbContext db) : IQueryHandler<DashboardQuery, DashboardResponse>
{
    public async Task<Result<DashboardResponse>> Handle(DashboardQuery query, CancellationToken cancellationToken)
    {
        // "Vencido" compara contra el día del cliente, no el del servidor: en un huso horario
        // detrás de UTC (Brasil/Uruguay), DateTime.UtcNow puede seguir en el día anterior al
        // del cliente, contando de menos (o de más, cerca de medianoche) instancias vencidas.
        var today = query.Today ?? DateOnly.FromDateTime(DateTime.UtcNow);

        var instancesQuery = db.ChecklistInstances.AsQueryable();

        if (query.FromDate is not null)
            instancesQuery = instancesQuery.Where(i => i.Date >= query.FromDate);

        if (query.ToDate is not null)
            instancesQuery = instancesQuery.Where(i => i.Date <= query.ToDate);

        var total = await instancesQuery.CountAsync(cancellationToken);
        var pending = await instancesQuery.CountAsync(i => i.Status == ChecklistStatus.Pending, cancellationToken);
        var approved = await instancesQuery.CountAsync(i => i.Status == ChecklistStatus.Approved, cancellationToken);
        var inProgress = await instancesQuery.CountAsync(i => i.Status == ChecklistStatus.InProgress, cancellationToken);
        var completed = await instancesQuery.CountAsync(i => i.Status == ChecklistStatus.Completed, cancellationToken);
        var reviewed = await instancesQuery.CountAsync(i => i.Status == ChecklistStatus.Reviewed, cancellationToken);
        var overdue = await instancesQuery.CountAsync(
            i => i.Date < today
                && (i.Status == ChecklistStatus.Pending || i.Status == ChecklistStatus.Approved || i.Status == ChecklistStatus.InProgress),
            cancellationToken);

        var averageDuration = await instancesQuery
            .Where(i => i.DurationSeconds != null)
            .Select(i => (double?)i.DurationSeconds!.Value)
            .AverageAsync(cancellationToken);

        var completionRate = total == 0 ? 0 : Math.Round((completed + reviewed) * 100.0 / total, 2);

        return Result.Success(new DashboardResponse(total, pending, approved, inProgress, completed, reviewed, overdue, averageDuration, completionRate));
    }
}
