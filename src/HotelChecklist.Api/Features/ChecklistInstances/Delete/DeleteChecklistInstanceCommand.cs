using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ChecklistInstances.Delete;

public sealed record DeleteChecklistInstanceCommand(Guid Id) : ICommand<Unit>;
