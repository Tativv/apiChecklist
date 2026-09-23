using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ChecklistInstances.AssignTask;

public sealed class AssignTaskHandler(AppDbContext db) : ICommandHandler<AssignTaskCommand, AssignTaskResponse>
{
    public async Task<Result<AssignTaskResponse>> Handle(AssignTaskCommand command, CancellationToken cancellationToken)
    {
        var taskExecution = await db.ChecklistTaskExecutions
            .FirstOrDefaultAsync(e => e.Id == command.TaskExecutionId && e.ChecklistInstanceId == command.InstanceId, cancellationToken);

        if (taskExecution is null)
            return Result.Failure<AssignTaskResponse>(Error.NotFound("ChecklistTaskExecutions.NotFound", "Tarea no encontrada."));

        var instance = await db.ChecklistInstances.FirstAsync(i => i.Id == command.InstanceId, cancellationToken);

        if (command.ActingUserIsExactlySupervisor)
        {
            var templateAreaId = await db.ChecklistTemplates
                .Where(t => t.Id == instance.TemplateId)
                .Select(t => t.AreaId)
                .FirstAsync(cancellationToken);

            var coversArea = await db.UserAreas.AnyAsync(
                ua => ua.UserId == command.ActingUserId && ua.AreaId == templateAreaId, cancellationToken);

            if (!coversArea)
                return Result.Failure<AssignTaskResponse>(
                    Error.Forbidden("ChecklistInstances.AreaNotCovered", "No supervisás el área de este checklist."));
        }

        if (command.UserId is not null)
        {
            var userExists = await db.Users.AnyAsync(u => u.Id == command.UserId && u.Active, cancellationToken);

            if (!userExists)
                return Result.Failure<AssignTaskResponse>(Error.NotFound("Users.NotFound", "Usuario no encontrado."));
        }

        taskExecution.AssignedUserId = command.UserId;
        taskExecution.CreatedByUserId = command.UserId is null ? null : command.ActingUserId;

        if (instance.Status is ChecklistStatus.Pending or ChecklistStatus.Approved)
        {
            var siblingAssignments = await db.ChecklistTaskExecutions
                .Where(e => e.ChecklistInstanceId == command.InstanceId)
                .Select(e => new { e.Id, e.AssignedUserId })
                .ToListAsync(cancellationToken);

            var allTasksAssigned = siblingAssignments.All(e =>
                e.Id == taskExecution.Id ? taskExecution.AssignedUserId is not null : e.AssignedUserId is not null);

            instance.Status = allTasksAssigned ? ChecklistStatus.Approved : ChecklistStatus.Pending;
        }

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(new AssignTaskResponse(taskExecution.Id, taskExecution.AssignedUserId, taskExecution.CreatedByUserId));
    }
}
