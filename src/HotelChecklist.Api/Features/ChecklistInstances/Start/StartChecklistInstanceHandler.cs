using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ChecklistInstances.Start;

public sealed class StartChecklistInstanceHandler(AppDbContext db) : ICommandHandler<StartChecklistInstanceCommand, StartChecklistInstanceResponse>
{
    public async Task<Result<StartChecklistInstanceResponse>> Handle(StartChecklistInstanceCommand command, CancellationToken cancellationToken)
    {
        var instance = await db.ChecklistInstances
            .Include(i => i.TaskExecutions)
            .FirstOrDefaultAsync(i => i.Id == command.Id, cancellationToken);

        if (instance is null)
            return Result.Failure<StartChecklistInstanceResponse>(Error.NotFound("ChecklistInstances.NotFound", "Checklist no encontrado."));

        if (instance.Status != ChecklistStatus.Pending)
            return Result.Failure<StartChecklistInstanceResponse>(
                Error.Conflict("ChecklistInstances.InvalidTransition", $"No se puede iniciar un checklist en estado {instance.Status}."));

        var hasAssignedTask = instance.TaskExecutions.Any(e => e.AssignedUserId == command.ActingUserId);

        if (!command.ActingUserIsSupervisorOrAbove && !hasAssignedTask)
            return Result.Failure<StartChecklistInstanceResponse>(
                Error.Forbidden("ChecklistInstances.NotAssigned", "Solo un colaborador con una tarea asignada acá o un supervisor pueden iniciar este checklist."));

        instance.Status = ChecklistStatus.InProgress;
        instance.StartedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(new StartChecklistInstanceResponse(instance.Id, instance.Status.ToString(), instance.StartedAt));
    }
}
