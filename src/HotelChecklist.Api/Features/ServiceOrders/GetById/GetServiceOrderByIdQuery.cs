using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ServiceOrders.GetById;

public sealed record GetServiceOrderByIdQuery(Guid Id) : IQuery<GetServiceOrderByIdResponse>;
