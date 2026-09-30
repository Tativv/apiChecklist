namespace HotelChecklist.Api.Features.ServiceOrders.Assign;

public static class AssignServiceOrderMapping
{
    public static AssignServiceOrderCommand ToCommand(
        this AssignServiceOrderRequest request, Guid serviceOrderId, Guid actingUserId, bool actingUserIsExactlySupervisor) =>
        new(serviceOrderId, request.UserId, actingUserId, actingUserIsExactlySupervisor);
}
