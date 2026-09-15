using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.Assets.GetById;

public sealed record GetAssetByIdQuery(Guid Id) : IQuery<GetAssetByIdResponse>;
