using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ServiceOrders.Finish;

public sealed record FinishServiceOrderCommand(Guid Id, Guid ActingUserId) : ICommand<FinishServiceOrderResponse>;
