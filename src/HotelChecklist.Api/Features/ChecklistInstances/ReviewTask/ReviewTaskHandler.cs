using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Api.Features.ChecklistInstances.TaskComments;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ChecklistInstances.ReviewTask;

public sealed class ReviewTaskHandler(AppDbContext db) : ICommandHandler<ReviewTaskCommand, ReviewTaskResponse>
{
    public async Task<Result<ReviewTaskResponse>> Handle(ReviewTaskCommand command, CancellationToken cancellationToken)
    {
        var taskExecution = await db.ChecklistTaskExecutions
            .FirstOrDefaultAsync(e => e.Id == command.TaskExecutionId && e.ChecklistInstanceId == command.InstanceId, cancellationToken);

        if (taskExecution is null)
            return Result.Failure<ReviewTaskResponse>(Error.NotFound("ChecklistTaskExecutions.NotFound", "Tarea no encontrada."));

        if (taskExecution.Status != TaskExecutionStatus.Completed)
            return Result.Failure<ReviewTaskResponse>(
                Error.Conflict("ChecklistTaskExecutions.InvalidTransition", $"No se puede revisar una tarea en estado {taskExecution.Status}."));

        taskExecution.Status = TaskExecutionStatus.Reviewed;
        taskExecution.ApprovedByUserId = command.ActingUserId;
        taskExecution.ApprovedAt = DateTimeOffset.UtcNow;

        SystemTaskCommentLog.Add(db, taskExecution.Id, command.ActingUserId, "Tarefa revisada.", taskExecution.ApprovedAt.Value);

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(new ReviewTaskResponse(taskExecution.Id, taskExecution.Status.ToString(), taskExecution.ApprovedByUserId, taskExecution.ApprovedAt));
    }
}
