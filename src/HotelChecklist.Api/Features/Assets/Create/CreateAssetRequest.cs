namespace HotelChecklist.Api.Features.Assets.Create;

public sealed record CreateAssetRequest(string Name, string Type, Guid AreaId);
