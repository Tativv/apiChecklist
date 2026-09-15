namespace HotelChecklist.Api.Features.Assets.GetById;

public sealed record GetAssetByIdResponse(Guid Id, string Name, string Type, Guid AreaId, bool Active);
