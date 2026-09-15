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
            .Include(t => t.Tasks)
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

        template.Name = command.Name;
        template.Description = command.Description;
        template.AreaId = command.AreaId;
        template.RecurrenceType = Enum.Parse<ChecklistRecurrenceType>(command.RecurrenceType, ignoreCase: true);
        template.EstimatedDurationMinutes = command.EstimatedDurationMinutes;

        template.Tasks.Clear();
        foreach (var task in command.Tasks)
            template.Tasks.Add(new ChecklistTask { Id = Guid.NewGuid(), Name = task.Name, Order = task.Order });

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(template.ToResponse());
    }
}
