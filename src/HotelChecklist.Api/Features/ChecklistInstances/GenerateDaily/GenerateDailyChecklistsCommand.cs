using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ChecklistInstances.GenerateDaily;

public sealed record GenerateDailyChecklistsCommand(DateOnly? Date) : ICommand<GenerateDailyChecklistsResponse>;
