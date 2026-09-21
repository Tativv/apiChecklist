using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ChecklistTemplates.Update;

public sealed class UpdateChecklistTemplateHandler(AppDbContext db) : ICommandHandler<UpdateChecklistTemplateCommand, UpdateChecklistTemplateResponse>
{
    public async Task<Result<UpdateChecklistTemplateResponse>> Handle(UpdateChecklistTemplateCommand command, CancellationToken cancellationToken)
    {
        var template = await db.ChecklistTemplates
            .Include(t => t.Tasks).ThenInclude(t => t.TaskSchedules)
            .Include(t => t.TemplateSchedules)
            .FirstOrDefaultAsync(t => t.Id == command.Id, cancellationToken);

        if (template is null)
            return Result.Failure<UpdateChecklistTemplateResponse>(Error.NotFound("ChecklistTemplates.NotFound", "Template no encontrado."));

        var areaExists = await db.Areas.AnyAsync(a => a.Id == command.AreaId, cancellationToken);

        if (!areaExists)
            return Result.Failure<UpdateChecklistTemplateResponse>(Error.NotFound("Areas.NotFound", "Área no encontrada."));

        var taskIds = template.Tasks.Select(t => t.Id).ToList();
        var hasExecutedTasks = await db.ChecklistTaskExecutions.AnyAsync(e => taskIds.Contains(e.TaskId), cancellationToken);

        if (hasExecutedTasks)
            return Result.Failure<UpdateChecklistTemplateResponse>(
                Error.Conflict("ChecklistTemplates.HasExecutions", "No se pueden modificar las tareas de un template con checklists ya ejecutados."));

        var orphanedScheduleIds = template.TemplateSchedules.Select(ts => ts.ScheduleId)
            .Concat(template.Tasks.SelectMany(t => t.TaskSchedules).Select(ts => ts.ScheduleId))
            .ToList();

        template.Name = command.Name;
        template.Description = command.Description;
        template.AreaId = command.AreaId;
        template.EstimatedDurationMinutes = command.EstimatedDurationMinutes;

        template.TemplateSchedules.Clear();
        foreach (var schedule in command.Schedules)
            template.TemplateSchedules.Add(new TemplateSchedule { Id = Guid.NewGuid(), Schedule = schedule.ToSchedule() });

        template.Tasks.Clear();
        foreach (var task in command.Tasks)
            template.Tasks.Add(task.ToTask());

        if (orphanedScheduleIds.Count > 0)
        {
            var orphaned = await db.Schedules.Where(s => orphanedScheduleIds.Contains(s.Id)).ToListAsync(cancellationToken);
            db.Schedules.RemoveRange(orphaned);
        }

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(template.ToResponse());
    }
}
