using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Entities;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ChecklistInstances;

public sealed class ChecklistInstanceCreationService(AppDbContext db, ScheduleEvaluationService evaluationService)
{
    public async Task<Result<ChecklistInstance>> CreateAsync(
        Guid templateId,
        Guid assetId,
        DateOnly date,
        CancellationToken cancellationToken)
    {
        var template = await db.ChecklistTemplates
            .Include(t => t.Tasks).ThenInclude(t => t.TaskSchedules).ThenInclude(ts => ts.Schedule)
            .Include(t => t.TemplateSchedules).ThenInclude(ts => ts.Schedule)
            .FirstOrDefaultAsync(t => t.Id == templateId, cancellationToken);

        if (template is null)
            return Result.Failure<ChecklistInstance>(Error.NotFound("ChecklistTemplates.NotFound", "Template no encontrado."));

        if (!evaluationService.ShouldExecuteTemplate(template, date))
            return Result.Failure<ChecklistInstance>(
                Error.Validation("ChecklistInstances.FrequencyMismatch", "La fecha seleccionada no coincide con la programación del template."));

        var assetExists = await db.Assets.AnyAsync(a => a.Id == assetId, cancellationToken);

        if (!assetExists)
            return Result.Failure<ChecklistInstance>(Error.NotFound("Assets.NotFound", "Activo no encontrado."));

        var alreadyExists = await db.ChecklistInstances
            .AnyAsync(i => i.TemplateId == templateId && i.AssetId == assetId && i.Date == date, cancellationToken);

        if (alreadyExists)
            return Result.Failure<ChecklistInstance>(
                Error.Conflict("ChecklistInstances.AlreadyExists", "Ya existe un checklist para ese activo, template y fecha."));

        var instance = new ChecklistInstance
        {
            Id = Guid.NewGuid(),
            TemplateId = templateId,
            AssetId = assetId,
            Date = date,
            Status = ChecklistStatus.Pending,
            TaskExecutions = template.Tasks.SelectMany(task => BuildExecutions(task, date)).ToList()
        };

        db.ChecklistInstances.Add(instance);

        return Result.Success(instance);
    }

    public static IEnumerable<ChecklistTaskExecution> BuildExecutions(ChecklistTask task, DateOnly date)
    {
        if (task.ExecutionMode == TaskExecutionMode.Continuous)
        {
            return [new ChecklistTaskExecution { Id = Guid.NewGuid(), TaskId = task.Id, Status = TaskExecutionStatus.Pending }];
        }

        return task.TaskSchedules.Select(ts => new ChecklistTaskExecution
        {
            Id = Guid.NewGuid(),
            TaskId = task.Id,
            ScheduleId = ts.ScheduleId,
            ScheduledForUtc = new DateTimeOffset(date.Year, date.Month, date.Day, ts.Schedule.TimeOfDay.Hour, ts.Schedule.TimeOfDay.Minute, 0, TimeSpan.Zero),
            Status = TaskExecutionStatus.Pending
        });
    }
}
