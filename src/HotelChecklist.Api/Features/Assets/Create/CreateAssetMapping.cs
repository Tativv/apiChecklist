using HotelChecklist.Domain.Entities;

namespace HotelChecklist.Api.Features.Assets.Create;

public static class CreateAssetMapping
{
    public static CreateAssetCommand ToCommand(this CreateAssetRequest request) => new(request.Name, request.Type, request.AreaId);

    public static CreateAssetResponse ToResponse(this Asset asset) => new(asset.Id, asset.Name, asset.Type, asset.AreaId, asset.Active);
}
