namespace HotelChecklist.Api.Features.Assets.List;

public sealed record ListAssetsResponseItem(Guid Id, string Name, string Type, Guid AreaId, bool Active);
