using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ChecklistInstances.GetMyAssignedTasks;

public sealed record GetMyAssignedTasksQuery(Guid ActingUserId, DateOnly Date) : IQuery<IReadOnlyList<MyAssignedTaskItem>>;
