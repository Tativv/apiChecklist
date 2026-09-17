using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Entities;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ChecklistTemplates.Create;

public sealed class CreateChecklistTemplateHandler(AppDbContext db) : ICommandHandler<CreateChecklistTemplateCommand, CreateChecklistTemplateResponse>
{
    public async Task<Result<CreateChecklistTemplateResponse>> Handle(CreateChecklistTemplateCommand command, CancellationToken cancellationToken)
    {
        var areaExists = await db.Areas.AnyAsync(a => a.Id == command.AreaId, cancellationToken);

        if (!areaExists)
            return Result.Failure<CreateChecklistTemplateResponse>(Error.NotFound("Areas.NotFound", "Área no encontrada."));

        var template = new ChecklistTemplate
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Description = command.Description,
            AreaId = command.AreaId,
            RecurrenceType = Enum.Parse<ChecklistRecurrenceType>(command.RecurrenceType, ignoreCase: true),
            EstimatedDurationMinutes = command.EstimatedDurationMinutes,
            Tasks = command.Tasks
                .Select(t => new ChecklistTask { Id = Guid.NewGuid(), Name = t.Name, Description = t.Description, Order = t.Order })
                .ToList()
        };

        ChecklistTemplateSchedulingMapping.ApplyScheduling(
            template,
            command.ScheduledTime,
            command.RecurrenceStartDate,
            command.CustomRecurrenceMode,
            command.RecurrenceIntervalValue,
            command.RecurrenceIntervalUnit,
            command.RecurrenceDaysOfWeek);

        db.ChecklistTemplates.Add(template);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(template.ToResponse());
    }
}
