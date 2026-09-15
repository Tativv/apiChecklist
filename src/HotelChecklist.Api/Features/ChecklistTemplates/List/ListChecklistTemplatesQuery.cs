using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ChecklistTemplates.List;

public sealed record ListChecklistTemplatesQuery(Guid? AreaId) : IQuery<IReadOnlyList<ListChecklistTemplatesResponseItem>>;
