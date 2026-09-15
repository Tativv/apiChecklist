using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ChecklistInstances.Create;

public sealed record CreateChecklistInstanceCommand(
    Guid TemplateId,
    Guid AssetId,
    DateOnly Date,
    Guid? AssignedUserId) : ICommand<CreateChecklistInstanceResponse>;
