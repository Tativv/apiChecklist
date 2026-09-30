namespace HotelChecklist.Api.Features.ServiceOrders.Create;

public sealed record CreateServiceOrderRequest(
    Guid AreaId,
    Guid AssetId,
    string Subject,
    string? Description,
    string Priority,
    DateTimeOffset DueAtUtc);
