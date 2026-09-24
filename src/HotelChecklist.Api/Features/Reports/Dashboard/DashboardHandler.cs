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
        var inProgress = await instancesQuery.CountAsync(i => i.Status == ChecklistStatus.InProgress, cancellationToken);
        var completed = await instancesQuery.CountAsync(i => i.Status == ChecklistStatus.Completed, cancellationToken);
        var overdue = await instancesQuery.CountAsync(
            i => i.Date < today && (i.Status == ChecklistStatus.Pending || i.Status == ChecklistStatus.InProgress),
            cancellationToken);

        var averageDuration = await instancesQuery
            .Where(i => i.DurationSeconds != null)
            .Select(i => (double?)i.DurationSeconds!.Value)
            .AverageAsync(cancellationToken);

        var completionRate = total == 0 ? 0 : Math.Round(completed * 100.0 / total, 2);

        var tasksQuery = db.ChecklistTaskExecutions.Where(e =>
            (query.FromDate == null || e.ChecklistInstance.Date >= query.FromDate)
            && (query.ToDate == null || e.ChecklistInstance.Date <= query.ToDate));

        var tasksTotal = await tasksQuery.CountAsync(cancellationToken);
        var tasksPending = await tasksQuery.CountAsync(e => e.Status == TaskExecutionStatus.Pending, cancellationToken);
        var tasksInProgress = await tasksQuery.CountAsync(e => e.Status == TaskExecutionStatus.InProgress, cancellationToken);
        var tasksCompleted = await tasksQuery.CountAsync(e => e.Status == TaskExecutionStatus.Completed, cancellationToken);
        var tasksReviewed = await tasksQuery.CountAsync(e => e.Status == TaskExecutionStatus.Reviewed, cancellationToken);
        var tasksOverdue = await tasksQuery.CountAsync(
            e => e.ChecklistInstance.Date < today && (e.Status == TaskExecutionStatus.Pending || e.Status == TaskExecutionStatus.InProgress),
            cancellationToken);

        var averageTaskDuration = await tasksQuery
            .Where(e => e.DurationSeconds != null)
            .Select(e => (double?)e.DurationSeconds!.Value)
            .AverageAsync(cancellationToken);

        return Result.Success(new DashboardResponse(
            total, pending, inProgress, completed, overdue, averageDuration, completionRate,
            tasksTotal, tasksPending, tasksInProgress, tasksCompleted, tasksReviewed, tasksOverdue, averageTaskDuration));
    }
}
