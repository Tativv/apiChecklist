namespace HotelChecklist.Api.Features.Assets.Update;

public sealed record UpdateAssetResponse(Guid Id, string Name, string Type, Guid AreaId, bool Active);
