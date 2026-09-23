using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ChecklistInstances.Approve;

public sealed class ApproveChecklistInstanceHandler(AppDbContext db) : ICommandHandler<ApproveChecklistInstanceCommand, ApproveChecklistInstanceResponse>
{
    public async Task<Result<ApproveChecklistInstanceResponse>> Handle(ApproveChecklistInstanceCommand command, CancellationToken cancellationToken)
    {
        var instance = await db.ChecklistInstances
            .Include(i => i.TaskExecutions)
            .FirstOrDefaultAsync(i => i.Id == command.Id, cancellationToken);

        if (instance is null)
            return Result.Failure<ApproveChecklistInstanceResponse>(Error.NotFound("ChecklistInstances.NotFound", "Checklist no encontrado."));

        if (instance.Status != ChecklistStatus.Completed)
            return Result.Failure<ApproveChecklistInstanceResponse>(
                Error.Conflict("ChecklistInstances.InvalidTransition", $"No se puede aprobar un checklist en estado {instance.Status}."));

        var approvedAt = DateTimeOffset.UtcNow;

        instance.Status = ChecklistStatus.Reviewed;

        foreach (var execution in instance.TaskExecutions.Where(e => e.Status == TaskExecutionStatus.Completed))
        {
            execution.ApprovedByUserId = command.ApprovedByUserId;
            execution.ApprovedAt = approvedAt;
        }

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(new ApproveChecklistInstanceResponse(instance.Id, instance.Status.ToString(), command.ApprovedByUserId, approvedAt));
    }
}
