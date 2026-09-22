using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ChecklistInstances.Reopen;

public sealed class ReopenChecklistInstanceHandler(AppDbContext db, ILogger<ReopenChecklistInstanceHandler> logger)
    : ICommandHandler<ReopenChecklistInstanceCommand, ReopenChecklistInstanceResponse>
{
    public async Task<Result<ReopenChecklistInstanceResponse>> Handle(ReopenChecklistInstanceCommand command, CancellationToken cancellationToken)
    {
        var instance = await db.ChecklistInstances
            .Include(i => i.TaskExecutions)
            .FirstOrDefaultAsync(i => i.Id == command.Id, cancellationToken);

        if (instance is null)
            return Result.Failure<ReopenChecklistInstanceResponse>(Error.NotFound("ChecklistInstances.NotFound", "Checklist no encontrado."));

        if (instance.Status is not (ChecklistStatus.Completed or ChecklistStatus.Approved))
            return Result.Failure<ReopenChecklistInstanceResponse>(
                Error.Conflict("ChecklistInstances.InvalidTransition", $"No se puede reabrir un checklist en estado {instance.Status}."));

        instance.Status = ChecklistStatus.InProgress;
        instance.CompletedAt = null;
        instance.DurationSeconds = null;

        foreach (var execution in instance.TaskExecutions)
        {
            execution.ApprovedByUserId = null;
            execution.ApprovedAt = null;
        }

        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation("ChecklistInstance {InstanceId} reopened. Reason: {Reason}", instance.Id, command.Reason ?? "(sin especificar)");

        return Result.Success(new ReopenChecklistInstanceResponse(instance.Id, instance.Status.ToString()));
    }
}
