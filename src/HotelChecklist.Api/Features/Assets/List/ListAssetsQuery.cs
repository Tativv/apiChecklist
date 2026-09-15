using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.Assets.List;

public sealed record ListAssetsQuery(Guid? AreaId, bool? Active) : IQuery<IReadOnlyList<ListAssetsResponseItem>>;
