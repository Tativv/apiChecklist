using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ChecklistInstances.Reopen;

public sealed record ReopenChecklistInstanceCommand(Guid Id, string? Reason) : ICommand<ReopenChecklistInstanceResponse>;
