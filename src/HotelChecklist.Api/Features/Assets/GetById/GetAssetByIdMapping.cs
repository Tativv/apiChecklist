using HotelChecklist.Domain.Entities;

namespace HotelChecklist.Api.Features.Assets.GetById;

public static class GetAssetByIdMapping
{
    public static GetAssetByIdResponse ToResponse(this Asset asset) => new(asset.Id, asset.Name, asset.Type, asset.AreaId, asset.Active);
}
