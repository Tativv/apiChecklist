using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ChecklistInstances.RestartTask;

public sealed record RestartTaskCommand(Guid InstanceId, Guid TaskExecutionId) : ICommand<RestartTaskResponse>;
