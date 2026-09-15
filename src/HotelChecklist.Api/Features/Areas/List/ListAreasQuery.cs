using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.Areas.List;

public sealed record ListAreasQuery : IQuery<IReadOnlyList<ListAreasResponseItem>>;
