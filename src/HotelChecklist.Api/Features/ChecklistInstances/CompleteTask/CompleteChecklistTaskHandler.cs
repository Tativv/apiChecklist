using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ChecklistInstances.CompleteTask;

public sealed class CompleteChecklistTaskHandler(AppDbContext db) : ICommandHandler<CompleteChecklistTaskCommand, CompleteChecklistTaskResponse>
{
    public async Task<Result<CompleteChecklistTaskResponse>> Handle(CompleteChecklistTaskCommand command, CancellationToken cancellationToken)
    {
        var instance = await db.ChecklistInstances.FindAsync([command.InstanceId], cancellationToken);

        if (instance is null)
            return Result.Failure<CompleteChecklistTaskResponse>(Error.NotFound("ChecklistInstances.NotFound", "Checklist no encontrado."));

        if (instance.Status != ChecklistStatus.InProgress)
            return Result.Failure<CompleteChecklistTaskResponse>(
                Error.Conflict("ChecklistInstances.InvalidTransition", "Solo se pueden completar tareas de un checklist en progreso."));

        var taskExecution = await db.ChecklistTaskExecutions
            .FirstOrDefaultAsync(e => e.Id == command.TaskExecutionId && e.ChecklistInstanceId == command.InstanceId, cancellationToken);

        if (taskExecution is null)
            return Result.Failure<CompleteChecklistTaskResponse>(Error.NotFound("ChecklistTaskExecutions.NotFound", "Tarea no encontrada."));

        if (taskExecution.Status != TaskExecutionStatus.InProgress)
            return Result.Failure<CompleteChecklistTaskResponse>(
                Error.Conflict("ChecklistTaskExecutions.InvalidTransition", $"No se puede concluir una tarea en estado {taskExecution.Status}."));

        var isOwnTask = taskExecution.AssignedUserId == command.ActingUserId;

        if (!command.ActingUserIsSupervisorOrAbove && !isOwnTask)
            return Result.Failure<CompleteChecklistTaskResponse>(
                Error.Forbidden("ChecklistTaskExecutions.NotAssigned", "Solo el colaborador asignado o un supervisor pueden concluir esta tarea."));

        taskExecution.Status = TaskExecutionStatus.Completed;
        taskExecution.CompletedAt = DateTimeOffset.UtcNow;
        taskExecution.DurationSeconds = taskExecution.StartedAt is null
            ? null
            : (long)(taskExecution.CompletedAt.Value - taskExecution.StartedAt.Value).TotalSeconds;
        taskExecution.ExecutedByUserId = command.ActingUserId;
        taskExecution.Comment = command.Comment;

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(new CompleteChecklistTaskResponse(
            taskExecution.Id, taskExecution.Status.ToString(), taskExecution.CompletedAt, taskExecution.DurationSeconds, taskExecution.Comment));
    }
}
