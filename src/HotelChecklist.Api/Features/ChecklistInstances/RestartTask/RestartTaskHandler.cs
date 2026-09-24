using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Api.Features.ChecklistInstances.TaskComments;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ChecklistInstances.RestartTask;

public sealed class RestartTaskHandler(AppDbContext db) : ICommandHandler<RestartTaskCommand, RestartTaskResponse>
{
    public async Task<Result<RestartTaskResponse>> Handle(RestartTaskCommand command, CancellationToken cancellationToken)
    {
        var taskExecution = await db.ChecklistTaskExecutions
            .FirstOrDefaultAsync(e => e.Id == command.TaskExecutionId && e.ChecklistInstanceId == command.InstanceId, cancellationToken);

        if (taskExecution is null)
            return Result.Failure<RestartTaskResponse>(Error.NotFound("ChecklistTaskExecutions.NotFound", "Tarea no encontrada."));

        if (taskExecution.Status is not (TaskExecutionStatus.Completed or TaskExecutionStatus.Reviewed))
            return Result.Failure<RestartTaskResponse>(
                Error.Conflict("ChecklistTaskExecutions.InvalidTransition", $"No se puede reiniciar una tarea en estado {taskExecution.Status}."));

        taskExecution.Status = TaskExecutionStatus.Pending;
        taskExecution.StartedAt = null;
        taskExecution.CompletedAt = null;
        taskExecution.DurationSeconds = null;
        taskExecution.ExecutedByUserId = null;
        taskExecution.ApprovedByUserId = null;
        taskExecution.ApprovedAt = null;

        var restartedAt = DateTimeOffset.UtcNow;
        SystemTaskCommentLog.Add(db, taskExecution.Id, command.ActingUserId, "Tarefa reiniciada.", restartedAt);

        // Un checklist Completed no puede seguir así si una de sus tareas vuelve a Pending —
        // rompería la invariante que exige FinishChecklistInstance (todas concluidas/revisadas).
        var instance = await db.ChecklistInstances.FirstAsync(i => i.Id == command.InstanceId, cancellationToken);
        if (instance.Status == ChecklistStatus.Completed)
        {
            instance.Status = ChecklistStatus.InProgress;
            instance.CompletedAt = null;
            instance.DurationSeconds = null;
        }

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(new RestartTaskResponse(taskExecution.Id, taskExecution.Status.ToString()));
    }
}
