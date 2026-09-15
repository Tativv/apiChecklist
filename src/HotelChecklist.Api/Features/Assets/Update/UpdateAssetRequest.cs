namespace HotelChecklist.Api.Features.Assets.Update;

public sealed record UpdateAssetRequest(string Name, string Type, Guid AreaId, bool Active);
