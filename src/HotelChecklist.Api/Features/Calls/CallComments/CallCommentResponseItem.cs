namespace HotelChecklist.Api.Features.Calls.CallComments;

public sealed record CallCommentResponseItem(
    Guid Id,
    Guid AuthorUserId,
    string AuthorName,
    DateTimeOffset CreatedAt,
    string? Text,
    string? FileName,
    string? ContentType);
