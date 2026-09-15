using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ChecklistTemplates.Delete;

public sealed class DeleteChecklistTemplateHandler(AppDbContext db) : ICommandHandler<DeleteChecklistTemplateCommand, Unit>
{
    public async Task<Result<Unit>> Handle(DeleteChecklistTemplateCommand command, CancellationToken cancellationToken)
    {
        var template = await db.ChecklistTemplates.FindAsync([command.Id], cancellationToken);

        if (template is null)
            return Result.Failure<Unit>(Error.NotFound("ChecklistTemplates.NotFound", "Template no encontrado."));

        var hasInstances = await db.ChecklistInstances.AnyAsync(i => i.TemplateId == command.Id, cancellationToken);

        if (hasInstances)
            return Result.Failure<Unit>(Error.Conflict("ChecklistTemplates.HasDependents", "No se puede eliminar un template con checklists asociados."));

        db.ChecklistTemplates.Remove(template);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}
