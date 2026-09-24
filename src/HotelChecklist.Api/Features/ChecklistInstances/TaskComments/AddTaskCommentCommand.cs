using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ChecklistInstances.TaskComments;

public sealed record AddTaskCommentCommand(
    Guid InstanceId,
    Guid TaskExecutionId,
    string? Text,
    Guid ActingUserId,
    Stream? FileContent,
    string? FileName,
    string? FileContentType,
    long? FileSizeBytes) : ICommand<TaskCommentResponseItem>;
