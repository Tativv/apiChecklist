using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ServiceOrders.Update;

public sealed record UpdateServiceOrderCommand(
    Guid Id,
    Guid AreaId,
    Guid AssetId,
    string Subject,
    string? Description,
    string Priority,
    DateTimeOffset DueAtUtc) : ICommand<UpdateServiceOrderResponse>;
