using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ChecklistTemplates.ApplyToAssets;

public sealed record ApplyTemplateToAssetsCommand(
    Guid TemplateId,
    List<Guid> AssetIds,
    DateOnly? Date,
    Guid? AssignedUserId) : ICommand<ApplyTemplateToAssetsResponse>;
