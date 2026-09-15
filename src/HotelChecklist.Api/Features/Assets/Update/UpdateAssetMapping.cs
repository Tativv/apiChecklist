using HotelChecklist.Domain.Entities;

namespace HotelChecklist.Api.Features.Assets.Update;

public static class UpdateAssetMapping
{
    public static UpdateAssetCommand ToCommand(this UpdateAssetRequest request, Guid id) =>
        new(id, request.Name, request.Type, request.AreaId, request.Active);

    public static UpdateAssetResponse ToResponse(this Asset asset) => new(asset.Id, asset.Name, asset.Type, asset.AreaId, asset.Active);
}
