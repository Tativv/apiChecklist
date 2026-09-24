using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ChecklistInstances.TaskComments;

public sealed class AddTaskCommentHandler(AppDbContext db) : ICommandHandler<AddTaskCommentCommand, TaskCommentResponseItem>
{
    public async Task<Result<TaskCommentResponseItem>> Handle(AddTaskCommentCommand command, CancellationToken cancellationToken)
    {
        var taskExists = await db.ChecklistTaskExecutions
            .AnyAsync(e => e.Id == command.TaskExecutionId && e.ChecklistInstanceId == command.InstanceId, cancellationToken);

        if (!taskExists)
            return Result.Failure<TaskCommentResponseItem>(Error.NotFound("ChecklistTaskExecutions.NotFound", "Tarea no encontrada."));

        var comment = new ChecklistTaskComment
        {
            Id = Guid.NewGuid(),
            ChecklistTaskExecutionId = command.TaskExecutionId,
            Text = command.Text,
            CreatedAt = DateTimeOffset.UtcNow,
            AuthorUserId = command.ActingUserId
        };

        db.ChecklistTaskComments.Add(comment);
        await db.SaveChangesAsync(cancellationToken);

        var authorName = await db.Users.Where(u => u.Id == command.ActingUserId).Select(u => u.Name).FirstAsync(cancellationToken);

        return Result.Success(new TaskCommentResponseItem(comment.Id, comment.AuthorUserId, authorName, comment.CreatedAt, comment.Text));
    }
}
