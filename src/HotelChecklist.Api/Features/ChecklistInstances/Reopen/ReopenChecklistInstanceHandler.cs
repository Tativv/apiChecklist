using HotelChecklist.Api.Common.Adapters.FileStorage;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ChecklistInstances.Reopen;

public sealed class ReopenChecklistInstanceHandler(AppDbContext db, IFileStorage fileStorage, ILogger<ReopenChecklistInstanceHandler> logger)
    : ICommandHandler<ReopenChecklistInstanceCommand, ReopenChecklistInstanceResponse>
{
    public async Task<Result<ReopenChecklistInstanceResponse>> Handle(ReopenChecklistInstanceCommand command, CancellationToken cancellationToken)
    {
        var instance = await db.ChecklistInstances
            .Include(i => i.TaskExecutions).ThenInclude(e => e.Evidences)
            .FirstOrDefaultAsync(i => i.Id == command.Id, cancellationToken);

        if (instance is null)
            return Result.Failure<ReopenChecklistInstanceResponse>(Error.NotFound("ChecklistInstances.NotFound", "Checklist no encontrado."));

        if (instance.Status is not (ChecklistStatus.InProgress or ChecklistStatus.Completed))
            return Result.Failure<ReopenChecklistInstanceResponse>(
                Error.Conflict("ChecklistInstances.InvalidTransition", $"No se puede reabrir un checklist en estado {instance.Status}."));

        instance.Status = ChecklistStatus.Pending;
        instance.StartedAt = null;
        instance.CompletedAt = null;
        instance.DurationSeconds = null;

        var evidences = instance.TaskExecutions.SelectMany(e => e.Evidences).ToList();

        foreach (var evidence in evidences)
            await fileStorage.DeleteAsync(evidence.FilePath, cancellationToken);

        db.ChecklistTaskEvidences.RemoveRange(evidences);

        foreach (var execution in instance.TaskExecutions)
        {
            execution.AssignedUserId = null;
            execution.CreatedByUserId = null;
            execution.EstimatedDurationMinutes = null;
            execution.Status = TaskExecutionStatus.Pending;
            execution.StartedAt = null;
            execution.CompletedAt = null;
            execution.DurationSeconds = null;
            execution.ExecutedByUserId = null;
            execution.ApprovedByUserId = null;
            execution.ApprovedAt = null;
            execution.Comment = null;
        }

        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation("ChecklistInstance {InstanceId} reopened. Reason: {Reason}", instance.Id, command.Reason ?? "(sin especificar)");

        return Result.Success(new ReopenChecklistInstanceResponse(instance.Id, instance.Status.ToString()));
    }
}
