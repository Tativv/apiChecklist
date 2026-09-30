using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ServiceOrders.Start;

public sealed record StartServiceOrderCommand(Guid Id, Guid ActingUserId) : ICommand<StartServiceOrderResponse>;
