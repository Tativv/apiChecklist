namespace HotelChecklist.Api.Features.ServiceOrders.Create;

public static class CreateServiceOrderMapping
{
    public static CreateServiceOrderCommand ToCommand(this CreateServiceOrderRequest request, Guid createdByUserId) =>
        new(request.AreaId, request.AssetId, request.Subject, request.Description, request.Priority, request.DueAtUtc, createdByUserId);
}
