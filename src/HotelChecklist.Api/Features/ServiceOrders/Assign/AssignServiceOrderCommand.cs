using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ServiceOrders.Assign;

public sealed record AssignServiceOrderCommand(
    Guid ServiceOrderId,
    Guid? UserId,
    Guid ActingUserId,
    bool ActingUserIsExactlySupervisor) : ICommand<AssignServiceOrderResponse>;
