using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.Areas.Delete;

public sealed record DeleteAreaCommand(Guid Id) : ICommand<Unit>;
