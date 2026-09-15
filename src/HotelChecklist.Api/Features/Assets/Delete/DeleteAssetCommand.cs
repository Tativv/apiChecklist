using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.Assets.Delete;

public sealed record DeleteAssetCommand(Guid Id) : ICommand<Unit>;
