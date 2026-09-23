using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.Calls.GetById;

public sealed record GetCallByIdQuery(Guid Id) : IQuery<GetCallByIdResponse>;
