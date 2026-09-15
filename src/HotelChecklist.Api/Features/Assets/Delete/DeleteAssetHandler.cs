using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.Assets.Delete;

public sealed class DeleteAssetHandler(AppDbContext db) : ICommandHandler<DeleteAssetCommand, Unit>
{
    public async Task<Result<Unit>> Handle(DeleteAssetCommand command, CancellationToken cancellationToken)
    {
        var asset = await db.Assets.FindAsync([command.Id], cancellationToken);

        if (asset is null)
            return Result.Failure<Unit>(Error.NotFound("Assets.NotFound", "Activo no encontrado."));

        var hasInstances = await db.ChecklistInstances.AnyAsync(i => i.AssetId == command.Id, cancellationToken);

        if (hasInstances)
            return Result.Failure<Unit>(Error.Conflict("Assets.HasDependents", "No se puede eliminar un activo con checklists asociados."));

        db.Assets.Remove(asset);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}
