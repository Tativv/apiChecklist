using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.Areas.GetById;

public sealed record GetAreaByIdQuery(Guid Id) : IQuery<GetAreaByIdResponse>;
