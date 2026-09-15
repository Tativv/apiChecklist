using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.Assets.Update;

public sealed class UpdateAssetHandler(AppDbContext db) : ICommandHandler<UpdateAssetCommand, UpdateAssetResponse>
{
    public async Task<Result<UpdateAssetResponse>> Handle(UpdateAssetCommand command, CancellationToken cancellationToken)
    {
        var asset = await db.Assets.FindAsync([command.Id], cancellationToken);

        if (asset is null)
            return Result.Failure<UpdateAssetResponse>(Error.NotFound("Assets.NotFound", "Activo no encontrado."));

        var areaExists = await db.Areas.AnyAsync(a => a.Id == command.AreaId, cancellationToken);

        if (!areaExists)
            return Result.Failure<UpdateAssetResponse>(Error.NotFound("Areas.NotFound", "Área no encontrada."));

        asset.Name = command.Name;
        asset.Type = command.Type;
        asset.AreaId = command.AreaId;
        asset.Active = command.Active;

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(asset.ToResponse());
    }
}
