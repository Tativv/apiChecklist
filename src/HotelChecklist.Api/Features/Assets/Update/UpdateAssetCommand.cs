using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.Assets.Update;

public sealed record UpdateAssetCommand(Guid Id, string Name, string Type, Guid AreaId, bool Active) : ICommand<UpdateAssetResponse>;
