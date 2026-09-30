namespace HotelChecklist.Api.Features.ServiceOrders.Update;

public static class UpdateServiceOrderMapping
{
    public static UpdateServiceOrderCommand ToCommand(this UpdateServiceOrderRequest request, Guid id) =>
        new(id, request.AreaId, request.AssetId, request.Subject, request.Description, request.Priority, request.DueAtUtc);
}
