using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Domain.Enums;

namespace HotelChecklist.Api.Features.ChecklistTemplates.ConfigureAssets;

public sealed record ConfigureTemplateAssetsCommand(Guid TemplateId, List<Guid> AssetIds, UserRole ActingUserRole) : ICommand<ConfigureTemplateAssetsResponse>;
