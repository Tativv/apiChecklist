namespace HotelChecklist.Api.Features.Assets.Create;

public sealed record CreateAssetResponse(Guid Id, string Name, string Type, Guid AreaId, bool Active);
