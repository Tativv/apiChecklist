using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ChecklistInstances.GetMyAssignedTasks;

public sealed class GetMyAssignedTasksHandler(AppDbContext db) : IQueryHandler<GetMyAssignedTasksQuery, IReadOnlyList<MyAssignedTaskItem>>
{
    public async Task<Result<IReadOnlyList<MyAssignedTaskItem>>> Handle(GetMyAssignedTasksQuery query, CancellationToken cancellationToken)
    {
        var items = await db.ChecklistTaskExecutions
            .Where(e => e.AssignedUserId == query.ActingUserId && e.ChecklistInstance.Date == query.Date)
            .OrderBy(e => e.ScheduledForUtc)
            .Select(e => new MyAssignedTaskItem(
                e.ChecklistInstanceId,
                e.ChecklistInstance.Template.Name,
                e.ChecklistInstance.AssetId,
                e.ChecklistInstance.Asset.Name,
                e.ChecklistInstance.Date,
                e.Id,
                e.Task.Name,
                e.ScheduledForUtc,
                e.Status.ToString()))
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<MyAssignedTaskItem>>(items);
    }
}
