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

        if (!RoleHierarchy.Outranks(command.ActingUserRole, template.CreatedByRole))
            return Result.Failure<Unit>(
                Error.Forbidden("ChecklistTemplates.InsufficientHierarchy", "Este template fue creado por un rol superior al tuyo."));

        var hasInstances = await db.ChecklistInstances.AnyAsync(i => i.TemplateId == command.Id, cancellationToken);

        if (hasInstances)
        {
            // No se puede borrar físicamente sin romper el FK de las instancias ya generadas:
            // se retira el template (deja de listarse y de generar checklists nuevos) sin tocar su histórico.
            template.IsSnapshot = true;
        }
        else
        {
            db.ChecklistTemplates.Remove(template);
        }

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}
