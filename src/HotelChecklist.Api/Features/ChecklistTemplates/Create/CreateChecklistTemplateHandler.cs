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

        var templateId = Guid.NewGuid();

        var template = new ChecklistTemplate
        {
            Id = templateId,
            GroupId = templateId,
            IsSnapshot = false,
            Name = command.Name,
            Description = command.Description,
            AreaId = command.AreaId,
            ExecutionMode = Enum.Parse<TaskExecutionMode>(command.ExecutionMode, ignoreCase: true),
            CreatedByRole = command.ActingUserRole,
            TemplateSchedules = command.Schedules
                .Select(s => new TemplateSchedule { Id = Guid.NewGuid(), Schedule = s.ToSchedule() })
                .ToList(),
            Tasks = command.Tasks.Select(t => t.ToTask()).ToList()
        };

        db.ChecklistTemplates.Add(template);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(template.ToResponse());
    }
}
