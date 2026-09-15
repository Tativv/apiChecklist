using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.Assets.Create;

public sealed record CreateAssetCommand(string Name, string Type, Guid AreaId) : ICommand<CreateAssetResponse>;
