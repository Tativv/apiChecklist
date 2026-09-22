using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ChecklistInstances.Finish;

public sealed record FinishChecklistInstanceCommand(Guid Id, Guid ActingUserId, bool ActingUserIsManagerOrAbove) : ICommand<FinishChecklistInstanceResponse>;
