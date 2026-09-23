using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ChecklistInstances.Start;

public sealed record StartChecklistInstanceCommand(Guid Id, Guid ActingUserId, bool ActingUserIsSupervisorOrAbove) : ICommand<StartChecklistInstanceResponse>;
