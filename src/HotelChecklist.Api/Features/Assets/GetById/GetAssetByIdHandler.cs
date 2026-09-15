using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;

namespace HotelChecklist.Api.Features.Assets.GetById;

public sealed class GetAssetByIdHandler(AppDbContext db) : IQueryHandler<GetAssetByIdQuery, GetAssetByIdResponse>
{
    public async Task<Result<GetAssetByIdResponse>> Handle(GetAssetByIdQuery query, CancellationToken cancellationToken)
    {
        var asset = await db.Assets.FindAsync([query.Id], cancellationToken);

        if (asset is null)
            return Result.Failure<GetAssetByIdResponse>(Error.NotFound("Assets.NotFound", "Activo no encontrado."));

        return Result.Success(asset.ToResponse());
    }
}
