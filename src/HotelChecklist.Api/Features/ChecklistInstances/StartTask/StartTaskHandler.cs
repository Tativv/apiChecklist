using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ChecklistInstances.StartTask;

public sealed class StartTaskHandler(AppDbContext db) : ICommandHandler<StartTaskCommand, StartTaskResponse>
{
    public async Task<Result<StartTaskResponse>> Handle(StartTaskCommand command, CancellationToken cancellationToken)
    {
        var taskExecution = await db.ChecklistTaskExecutions
            .FirstOrDefaultAsync(e => e.Id == command.TaskExecutionId && e.ChecklistInstanceId == command.InstanceId, cancellationToken);

        if (taskExecution is null)
            return Result.Failure<StartTaskResponse>(Error.NotFound("ChecklistTaskExecutions.NotFound", "Tarea no encontrada."));

        if (taskExecution.Status != TaskExecutionStatus.Pending)
            return Result.Failure<StartTaskResponse>(
                Error.Conflict("ChecklistTaskExecutions.InvalidTransition", $"No se puede iniciar una tarea en estado {taskExecution.Status}."));

        var isOwnTask = taskExecution.AssignedUserId == command.ActingUserId;

        if (!command.ActingUserIsSupervisorOrAbove && !isOwnTask)
            return Result.Failure<StartTaskResponse>(
                Error.Forbidden("ChecklistTaskExecutions.NotAssigned", "Solo el colaborador asignado o un supervisor pueden iniciar esta tarea."));

        taskExecution.Status = TaskExecutionStatus.InProgress;
        taskExecution.StartedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(new StartTaskResponse(taskExecution.Id, taskExecution.Status.ToString(), taskExecution.StartedAt));
    }
}
