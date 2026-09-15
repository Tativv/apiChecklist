using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ChecklistTemplates.GetById;

public sealed record GetChecklistTemplateByIdQuery(Guid Id) : IQuery<GetChecklistTemplateByIdResponse>;
