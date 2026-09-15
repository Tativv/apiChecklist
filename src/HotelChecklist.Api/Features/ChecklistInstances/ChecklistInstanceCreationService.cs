using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Entities;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ChecklistInstances;

public sealed class ChecklistInstanceCreationService(AppDbContext db)
{
    public async Task<Result<ChecklistInstance>> CreateAsync(
        Guid templateId,
        Guid assetId,
        DateOnly date,
        Guid? assignedUserId,
        CancellationToken cancellationToken)
    {
        var template = await db.ChecklistTemplates
            .Include(t => t.Tasks)
            .FirstOrDefaultAsync(t => t.Id == templateId, cancellationToken);

        if (template is null)
            return Result.Failure<ChecklistInstance>(Error.NotFound("ChecklistTemplates.NotFound", "Template no encontrado."));

        var assetExists = await db.Assets.AnyAsync(a => a.Id == assetId, cancellationToken);

        if (!assetExists)
            return Result.Failure<ChecklistInstance>(Error.NotFound("Assets.NotFound", "Activo no encontrado."));

        if (assignedUserId is not null)
        {
            var userExists = await db.Users.AnyAsync(u => u.Id == assignedUserId && u.Active, cancellationToken);

            if (!userExists)
                return Result.Failure<ChecklistInstance>(Error.NotFound("Users.NotFound", "Usuario asignado no encontrado."));
        }

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
            AssignedUserId = assignedUserId,
            TaskExecutions = template.Tasks
                .Select(t => new ChecklistTaskExecution { Id = Guid.NewGuid(), TaskId = t.Id, Completed = false })
                .ToList()
        };

        db.ChecklistInstances.Add(instance);

        return Result.Success(instance);
    }
}
