namespace HotelChecklist.Api.Features.ServiceOrders.Finish;

public sealed record FinishServiceOrderResponse(Guid Id, string Status, DateTimeOffset? CompletedAt, long? DurationSeconds);
