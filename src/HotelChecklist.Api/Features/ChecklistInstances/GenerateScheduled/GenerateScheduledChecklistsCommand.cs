using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ChecklistInstances.GenerateScheduled;

public sealed record GenerateScheduledChecklistsCommand(DateOnly? Date) : ICommand<GenerateScheduledChecklistsResponse>;
