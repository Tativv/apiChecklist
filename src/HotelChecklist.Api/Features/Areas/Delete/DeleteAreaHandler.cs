using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.Areas.Delete;

public sealed class DeleteAreaHandler(AppDbContext db) : ICommandHandler<DeleteAreaCommand, Unit>
{
    public async Task<Result<Unit>> Handle(DeleteAreaCommand command, CancellationToken cancellationToken)
    {
        var area = await db.Areas.FindAsync([command.Id], cancellationToken);

        if (area is null)
            return Result.Failure<Unit>(Error.NotFound("Areas.NotFound", "Área no encontrada."));

        var hasDependents = await db.Assets.AnyAsync(a => a.AreaId == command.Id, cancellationToken)
            || await db.ChecklistTemplates.AnyAsync(t => t.AreaId == command.Id, cancellationToken);

        if (hasDependents)
            return Result.Failure<Unit>(Error.Conflict("Areas.HasDependents", "No se puede eliminar un área con activos o templates asociados."));

        db.Areas.Remove(area);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}
