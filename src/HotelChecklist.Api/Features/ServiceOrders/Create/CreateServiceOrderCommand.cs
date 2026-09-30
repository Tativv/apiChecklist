using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ServiceOrders.Create;

public sealed record CreateServiceOrderCommand(
    Guid AreaId,
    Guid AssetId,
    string Subject,
    string? Description,
    string Priority,
    DateTimeOffset DueAtUtc,
    Guid CreatedByUserId) : ICommand<CreateServiceOrderResponse>;
