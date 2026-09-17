using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ChecklistTemplates.ConfigureAssets;

public sealed record ConfigureTemplateAssetsCommand(Guid TemplateId, List<Guid> AssetIds) : ICommand<ConfigureTemplateAssetsResponse>;
