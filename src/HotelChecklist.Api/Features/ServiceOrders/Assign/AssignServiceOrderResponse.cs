namespace HotelChecklist.Api.Features.ServiceOrders.Assign;

public sealed record AssignServiceOrderResponse(Guid Id, Guid? AssignedUserId, string? AssignedUserName);
