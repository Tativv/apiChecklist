using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Api.Features.ChecklistInstances;
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
            .Include(t => t.Tasks).ThenInclude(t => t.TaskSchedules)
            .Include(t => t.TemplateSchedules)
            .Include(t => t.TemplateAssets)
            .FirstOrDefaultAsync(t => t.Id == command.Id, cancellationToken);

        if (template is null)
            return Result.Failure<UpdateChecklistTemplateResponse>(Error.NotFound("ChecklistTemplates.NotFound", "Template no encontrado."));

        if (template.IsSnapshot)
            return Result.Failure<UpdateChecklistTemplateResponse>(
                Error.Conflict("ChecklistTemplates.IsSnapshot", "No se puede editar una versión histórica de un template."));

        var areaExists = await db.Areas.AnyAsync(a => a.Id == command.AreaId, cancellationToken);

        if (!areaExists)
            return Result.Failure<UpdateChecklistTemplateResponse>(Error.NotFound("Areas.NotFound", "Área no encontrada."));

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var taskIds = template.Tasks.Select(t => t.Id).ToList();
        var relatedExecutions = await db.ChecklistTaskExecutions
            .Where(e => taskIds.Contains(e.TaskId))
            .Include(e => e.ChecklistInstance)
            .ToListAsync(cancellationToken);

        var hasPastExecutions = relatedExecutions.Any(e => e.ChecklistInstance.Date != today);

        if (!hasPastExecutions)
        {
            var todayInstances = relatedExecutions
                .Select(e => e.ChecklistInstance)
                .DistinctBy(i => i.Id)
                .ToList();

            if (todayInstances.Any(i => i.Status is ChecklistStatus.Completed or ChecklistStatus.Reviewed))
                return Result.Failure<UpdateChecklistTemplateResponse>(
                    Error.Conflict("ChecklistTemplates.TodayInstanceNotReopened",
                        "El checklist de hoy para este template ya fue finalizado. Reabrilo antes de modificar el template."));

            var orphanedScheduleIds = template.TemplateSchedules.Select(ts => ts.ScheduleId)
                .Concat(template.Tasks.SelectMany(t => t.TaskSchedules).Select(ts => ts.ScheduleId))
                .ToList();

            template.Name = command.Name;
            template.Description = command.Description;
            template.AreaId = command.AreaId;
            template.EstimatedDurationMinutes = command.EstimatedDurationMinutes;

            // Las ejecuciones de hoy tienen que borrarse ANTES que sus ChecklistTask: la FK
            // ChecklistTaskExecution.TaskId es Restrict, así que EF revienta apenas se marca el
            // Task como eliminado si sus ejecuciones todavía están "vivas" en el change tracker.
            // Quedan huérfanas de sus tareas viejas: se eliminan y se regeneran en Pending para
            // las tareas nuevas, reseteando la(s) instancia(s) de hoy a Pending — el checklist de
            // hoy "empieza de nuevo" con la definición editada.
            db.ChecklistTaskExecutions.RemoveRange(relatedExecutions);

            // Reemplazo vía RemoveRange/AddRange sobre el DbSet en vez de Clear()+Add() sobre la
            // navegación: Clear() en una colección ya trackeada con hijos anidados (TaskSchedules)
            // hace que el change tracker de EF Core a veces traduzca un DELETE+INSERT como un
            // UPDATE sobre la fila vieja con los datos nuevos, lo que revienta con
            // DbUpdateConcurrencyException apenas hay más de una fila de por medio (mismo patrón
            // que UpdateUserHandler con UserAreas).
            db.TemplateSchedules.RemoveRange(template.TemplateSchedules);
            db.ChecklistTasks.RemoveRange(template.Tasks);

            foreach (var instance in todayInstances)
            {
                instance.Status = ChecklistStatus.Pending;
                instance.StartedAt = null;
                instance.CompletedAt = null;
                instance.DurationSeconds = null;
            }

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

            var newExecutions = todayInstances.SelectMany(instance =>
                newTasks.SelectMany(task => ChecklistInstanceCreationService.BuildExecutions(task, instance.Date))
                    .Select(execution =>
                    {
                        execution.ChecklistInstanceId = instance.Id;
                        return execution;
                    }));
            db.ChecklistTaskExecutions.AddRange(newExecutions);

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

        // El template ya tiene checklists ejecutados en días anteriores: no se puede reemplazar
        // sus tareas in situ (borrarlas dispararía cascade delete sobre esas ChecklistTaskExecution
        // históricas). Se congela la fila actual tal cual está y se crea una nueva versión "viva"
        // del mismo grupo.
        template.IsSnapshot = true;

        var newVersion = new ChecklistTemplate
        {
            Id = Guid.NewGuid(),
            GroupId = template.GroupId,
            IsSnapshot = false,
            Name = command.Name,
            Description = command.Description,
            AreaId = command.AreaId,
            EstimatedDurationMinutes = command.EstimatedDurationMinutes,
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
}
