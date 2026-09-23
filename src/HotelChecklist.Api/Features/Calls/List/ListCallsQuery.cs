using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.Calls.List;

public sealed record ListCallsQuery(
    Guid? AreaId,
    string? Status,
    string? Priority,
    Guid? AssignedUserId,
    Guid ActingUserId,
    bool RestrictToSupervisedAreas,
    bool RestrictToOwnOrUnassignedInAreas) : IQuery<IReadOnlyList<ListCallsResponseItem>>;
