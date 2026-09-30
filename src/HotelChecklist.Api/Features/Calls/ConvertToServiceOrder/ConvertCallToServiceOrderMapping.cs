namespace HotelChecklist.Api.Features.Calls.ConvertToServiceOrder;

public static class ConvertCallToServiceOrderMapping
{
    public static ConvertCallToServiceOrderCommand ToCommand(this ConvertCallToServiceOrderRequest request, Guid callId, Guid actingUserId) =>
        new(callId, request.AssetId, request.DueAtUtc, request.Priority, request.Subject, request.Description, actingUserId);
}
