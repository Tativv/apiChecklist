using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ServiceOrders.List;

public sealed record ListServiceOrdersQuery(
    Guid? AreaId,
    Guid? AssetId,
    string? Status,
    string? Priority,
    Guid? AssignedUserId,
    Guid ActingUserId,
    bool RestrictToSupervisedAreas,
    bool RestrictToOwnOrUnassignedInAreas) : IQuery<IReadOnlyList<ListServiceOrdersResponseItem>>;
