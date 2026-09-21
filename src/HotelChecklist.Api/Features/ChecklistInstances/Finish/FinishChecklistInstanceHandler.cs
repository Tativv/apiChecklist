using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ChecklistInstances.Finish;

public sealed class FinishChecklistInstanceHandler(AppDbContext db) : ICommandHandler<FinishChecklistInstanceCommand, FinishChecklistInstanceResponse>
{
    public async Task<Result<FinishChecklistInstanceResponse>> Handle(FinishChecklistInstanceCommand command, CancellationToken cancellationToken)
    {
        var instance = await db.ChecklistInstances
            .Include(i => i.TaskExecutions)
            .FirstOrDefaultAsync(i => i.Id == command.Id, cancellationToken);

        if (instance is null)
            return Result.Failure<FinishChecklistInstanceResponse>(Error.NotFound("ChecklistInstances.NotFound", "Checklist no encontrado."));

        if (instance.Status != ChecklistStatus.InProgress)
            return Result.Failure<FinishChecklistInstanceResponse>(
                Error.Conflict("ChecklistInstances.InvalidTransition", $"No se puede finalizar un checklist en estado {instance.Status}."));

        if (instance.AssignedUserId != command.ActingUserId && !command.ActingUserIsSupervisorOrAbove)
            return Result.Failure<FinishChecklistInstanceResponse>(
                Error.Forbidden("ChecklistInstances.NotAssigned", "Solo el usuario asignado o un supervisor pueden finalizar este checklist."));

        if (instance.TaskExecutions.Any(e => e.Status == TaskExecutionStatus.Pending))
            return Result.Failure<FinishChecklistInstanceResponse>(
                Error.Validation("ChecklistInstances.PendingTasks", "Todas las tareas deben estar completadas antes de finalizar."));

        instance.CompletedAt = DateTimeOffset.UtcNow;
        instance.DurationSeconds = (long)(instance.CompletedAt.Value - instance.StartedAt!.Value).TotalSeconds;
        instance.Status = ChecklistStatus.Completed;

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(new FinishChecklistInstanceResponse(instance.Id, instance.Status.ToString(), instance.CompletedAt, instance.DurationSeconds));
    }
}
