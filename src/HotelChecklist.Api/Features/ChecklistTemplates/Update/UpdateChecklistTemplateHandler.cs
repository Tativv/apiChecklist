using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Entities;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ChecklistTemplates.Update;

public sealed class UpdateChecklistTemplateHandler(AppDbContext db) : ICommandHandler<UpdateChecklistTemplateCommand, UpdateChecklistTemplateResponse>
{
    public async Task<Result<UpdateChecklistTemplateResponse>> Handle(UpdateChecklistTemplateCommand command, CancellationToken cancellationToken)
    {
        var template = await db.ChecklistTemplates
            .Include(t => t.Tasks).ThenInclude(t => t.TaskSchedules).ThenInclude(ts => ts.Schedule)
            .Include(t => t.TemplateSchedules)
            .Include(t => t.TemplateAssets)
            .FirstOrDefaultAsync(t => t.Id == command.Id, cancellationToken);

        if (template is null)
            return Result.Failure<UpdateChecklistTemplateResponse>(Error.NotFound("ChecklistTemplates.NotFound", "Template no encontrado."));

        if (template.IsSnapshot)
            return Result.Failure<UpdateChecklistTemplateResponse>(
                Error.Conflict("ChecklistTemplates.IsSnapshot", "No se puede editar una versión histórica de un template."));

        if (!RoleHierarchy.Outranks(command.ActingUserRole, template.CreatedByRole))
            return Result.Failure<UpdateChecklistTemplateResponse>(
                Error.Forbidden("ChecklistTemplates.InsufficientHierarchy", "Este template fue creado por un rol superior al tuyo."));

        var areaExists = await db.Areas.AnyAsync(a => a.Id == command.AreaId, cancellationToken);

        if (!areaExists)
            return Result.Failure<UpdateChecklistTemplateResponse>(Error.NotFound("Areas.NotFound", "Área no encontrada."));

        var tasksChanged = TasksChanged(template.Tasks, command.Tasks);

        if (!tasksChanged)
        {
            // Nombre/descripción/área/duración/horarios del template no tienen ninguna relación
            // con ChecklistTask ni con ChecklistTaskExecution: siempre es seguro mutarlos en el
            // lugar, sin importar si el template ya generó checklists (hoy o en el pasado).
            var orphanedScheduleIds = template.TemplateSchedules.Select(ts => ts.ScheduleId).ToList();

            template.Name = command.Name;
            template.Description = command.Description;
            template.AreaId = command.AreaId;
            template.ExecutionMode = Enum.Parse<TaskExecutionMode>(command.ExecutionMode, ignoreCase: true);

            db.TemplateSchedules.RemoveRange(template.TemplateSchedules);
            var newTemplateSchedules = command.Schedules
                .Select(s => new TemplateSchedule { Id = Guid.NewGuid(), TemplateId = template.Id, Schedule = s.ToSchedule() })
                .ToList();
            db.TemplateSchedules.AddRange(newTemplateSchedules);

            if (orphanedScheduleIds.Count > 0)
            {
                var orphaned = await db.Schedules.Where(s => orphanedScheduleIds.Contains(s.Id)).ToListAsync(cancellationToken);
                db.Schedules.RemoveRange(orphaned);
            }

            await db.SaveChangesAsync(cancellationToken);

            template.TemplateSchedules = newTemplateSchedules;

            return Result.Success(template.ToResponse(versionedAsNewTemplate: false));
        }

        // La lista de tareas cambió: eso sí toca ChecklistTask, y sus ChecklistTaskExecution
        // tienen FK Restrict. Si el template nunca generó ninguna ejecución, reemplazar in situ
        // es seguro. En cuanto generó al menos una —de hoy o de cualquier fecha anterior—, se
        // versiona siempre: se congela la fila actual (intacta, con sus tareas viejas) y se crea
        // una nueva versión viva. Un mismo template puede tener listas de tareas distintas según
        // el momento, igual que puede tener distintos activos asociados.
        var taskIds = template.Tasks.Select(t => t.Id).ToList();
        var hasAnyExecution = await db.ChecklistTaskExecutions.AnyAsync(e => taskIds.Contains(e.TaskId), cancellationToken);

        if (!hasAnyExecution)
        {
            var orphanedScheduleIds = template.TemplateSchedules.Select(ts => ts.ScheduleId)
                .Concat(template.Tasks.SelectMany(t => t.TaskSchedules).Select(ts => ts.ScheduleId))
                .ToList();

            template.Name = command.Name;
            template.Description = command.Description;
            template.AreaId = command.AreaId;
            template.ExecutionMode = Enum.Parse<TaskExecutionMode>(command.ExecutionMode, ignoreCase: true);

            // Reemplazo vía RemoveRange/AddRange sobre el DbSet en vez de Clear()+Add() sobre la
            // navegación: Clear() en una colección ya trackeada con hijos anidados (TaskSchedules)
            // hace que el change tracker de EF Core a veces traduzca un DELETE+INSERT como un
            // UPDATE sobre la fila vieja con los datos nuevos, lo que revienta con
            // DbUpdateConcurrencyException apenas hay más de una fila de por medio (mismo patrón
            // que UpdateUserHandler con UserAreas).
            db.TemplateSchedules.RemoveRange(template.TemplateSchedules);
            db.ChecklistTasks.RemoveRange(template.Tasks);

            var newSchedules = command.Schedules
                .Select(s => new TemplateSchedule { Id = Guid.NewGuid(), TemplateId = template.Id, Schedule = s.ToSchedule() })
                .ToList();
            var newTasks = command.Tasks.Select(t =>
            {
                var task = t.ToTask();
                task.TemplateId = template.Id;
                return task;
            }).ToList();

            db.TemplateSchedules.AddRange(newSchedules);
            db.ChecklistTasks.AddRange(newTasks);

            if (orphanedScheduleIds.Count > 0)
            {
                var orphaned = await db.Schedules.Where(s => orphanedScheduleIds.Contains(s.Id)).ToListAsync(cancellationToken);
                db.Schedules.RemoveRange(orphaned);
            }

            await db.SaveChangesAsync(cancellationToken);

            template.TemplateSchedules = newSchedules;
            template.Tasks = newTasks;

            return Result.Success(template.ToResponse(versionedAsNewTemplate: false));
        }

        template.IsSnapshot = true;

        var newVersion = new ChecklistTemplate
        {
            Id = Guid.NewGuid(),
            GroupId = template.GroupId,
            IsSnapshot = false,
            Name = command.Name,
            Description = command.Description,
            AreaId = command.AreaId,
            ExecutionMode = Enum.Parse<TaskExecutionMode>(command.ExecutionMode, ignoreCase: true),
            CreatedByRole = command.ActingUserRole,
            TemplateSchedules = command.Schedules
                .Select(s => new TemplateSchedule { Id = Guid.NewGuid(), Schedule = s.ToSchedule() })
                .ToList(),
            Tasks = command.Tasks.Select(t => t.ToTask()).ToList(),
            TemplateAssets = template.TemplateAssets
                .Select(ta => new TemplateAsset { Id = Guid.NewGuid(), AssetId = ta.AssetId, CreatedAtUtc = DateTimeOffset.UtcNow })
                .ToList()
        };

        db.ChecklistTemplates.Add(newVersion);

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(newVersion.ToResponse(versionedAsNewTemplate: true));
    }

    private static bool TasksChanged(ICollection<ChecklistTask> oldTasks, IReadOnlyCollection<ChecklistTaskRequest> newTasks)
    {
        if (oldTasks.Count != newTasks.Count)
            return true;

        var oldByOrder = oldTasks.ToDictionary(t => t.Order);

        foreach (var newTask in newTasks)
        {
            if (!oldByOrder.TryGetValue(newTask.Order, out var oldTask))
                return true;

            if (oldTask.Name != newTask.Name
                || oldTask.Description != newTask.Description
                || oldTask.EstimatedDurationMinutes != newTask.EstimatedDurationMinutes
                || !string.Equals(oldTask.ExecutionMode.ToString(), newTask.ExecutionMode, StringComparison.OrdinalIgnoreCase))
                return true;

            if (SchedulesChanged(oldTask.TaskSchedules, newTask.Schedules))
                return true;
        }

        return false;
    }

    private static bool SchedulesChanged(ICollection<TaskSchedule> oldSchedules, IReadOnlyCollection<ScheduleInput> newSchedules)
    {
        if (oldSchedules.Count != newSchedules.Count)
            return true;

        var oldNormalized = oldSchedules
            .Select(ts => (
                FrequencyType: ts.Schedule.FrequencyType.ToString(),
                ts.Schedule.IntervalValue,
                WeekDay: ts.Schedule.WeekDay?.ToString(),
                ts.Schedule.DayOfMonth,
                TimeOfDay: ScheduleMapping.FormatTimeOfDay(ts.Schedule.TimeOfDay)))
            .OrderBy(x => x.FrequencyType).ThenBy(x => x.TimeOfDay).ThenBy(x => x.WeekDay).ThenBy(x => x.DayOfMonth)
            .ToList();

        var newNormalized = newSchedules
            .Select(s => (
                FrequencyType: s.FrequencyType,
                s.IntervalValue,
                WeekDay: s.WeekDay,
                s.DayOfMonth,
                TimeOfDay: s.TimeOfDay))
            .OrderBy(x => x.FrequencyType).ThenBy(x => x.TimeOfDay).ThenBy(x => x.WeekDay).ThenBy(x => x.DayOfMonth)
            .ToList();

        return !oldNormalized.SequenceEqual(newNormalized);
    }
}
