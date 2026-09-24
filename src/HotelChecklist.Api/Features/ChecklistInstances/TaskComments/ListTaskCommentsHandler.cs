using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ChecklistInstances.TaskComments;

public sealed class ListTaskCommentsHandler(AppDbContext db) : IQueryHandler<ListTaskCommentsQuery, IReadOnlyList<TaskCommentResponseItem>>
{
    public async Task<Result<IReadOnlyList<TaskCommentResponseItem>>> Handle(ListTaskCommentsQuery query, CancellationToken cancellationToken)
    {
        var taskExists = await db.ChecklistTaskExecutions
            .AnyAsync(e => e.Id == query.TaskExecutionId && e.ChecklistInstanceId == query.InstanceId, cancellationToken);

        if (!taskExists)
            return Result.Failure<IReadOnlyList<TaskCommentResponseItem>>(Error.NotFound("ChecklistTaskExecutions.NotFound", "Tarea no encontrada."));

        var comments = await db.ChecklistTaskComments
            .Where(c => c.ChecklistTaskExecutionId == query.TaskExecutionId)
            .OrderBy(c => c.CreatedAt)
            .Select(c => new TaskCommentResponseItem(c.Id, c.AuthorUserId, c.AuthorUser.Name, c.CreatedAt, c.Text, c.FileName, c.ContentType))
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<TaskCommentResponseItem>>(comments);
    }
}
