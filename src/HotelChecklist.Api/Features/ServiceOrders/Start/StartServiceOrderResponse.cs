namespace HotelChecklist.Api.Features.ServiceOrders.Start;

public sealed record StartServiceOrderResponse(Guid Id, string Status, DateTimeOffset? StartedAt);
