using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ServiceOrders.Delete;

public sealed record DeleteServiceOrderCommand(Guid Id) : ICommand<Unit>;
