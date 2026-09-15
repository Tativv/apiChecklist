using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ChecklistInstances.GetById;

public sealed record GetChecklistInstanceByIdQuery(Guid Id) : IQuery<GetChecklistInstanceByIdResponse>;
