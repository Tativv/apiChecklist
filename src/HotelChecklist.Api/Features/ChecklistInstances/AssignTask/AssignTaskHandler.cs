using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ChecklistInstances.AssignTask;

public sealed class AssignTaskHandler(AppDbContext db) : ICommandHandler<AssignTaskCommand, AssignTaskResponse>
{
    public async Task<Result<AssignTaskResponse>> Handle(AssignTaskCommand command, CancellationToken cancellationToken)
    {
        var taskExecution = await db.ChecklistTaskExecutions
            .Include(e => e.ChecklistInstance).ThenInclude(i => i.Template)
            .FirstOrDefaultAsync(e => e.Id == command.TaskExecutionId && e.ChecklistInstanceId == command.InstanceId, cancellationToken);

        if (taskExecution is null)
            return Result.Failure<AssignTaskResponse>(Error.NotFound("ChecklistTaskExecutions.NotFound", "Tarea no encontrada."));

        if (command.ActingUserIsExactlySupervisor)
        {
            var coversArea = await db.UserAreas.AnyAsync(
                ua => ua.UserId == command.ActingUserId && ua.AreaId == taskExecution.ChecklistInstance.Template.AreaId, cancellationToken);

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

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(new AssignTaskResponse(taskExecution.Id, taskExecution.AssignedUserId, taskExecution.CreatedByUserId));
    }
}
