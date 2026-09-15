using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ChecklistInstances.List;

public sealed record ListChecklistInstancesQuery(
    DateOnly? FromDate,
    DateOnly? ToDate,
    Guid? AreaId,
    Guid? AssetId,
    string? Status,
    Guid? AssignedUserId) : IQuery<IReadOnlyList<ListChecklistInstancesResponseItem>>;
