namespace HotelChecklist.Api.Features.ChecklistInstances.TaskComments;

public sealed record TaskCommentResponseItem(
    Guid Id,
    Guid AuthorUserId,
    string AuthorName,
    DateTimeOffset CreatedAt,
    string? Text,
    string? FileName,
    string? ContentType);
