namespace HotelChecklist.Api.Features.ServiceOrders.ServiceOrderComments;

public sealed record ServiceOrderCommentResponseItem(
    Guid Id,
    Guid AuthorUserId,
    string AuthorName,
    DateTimeOffset CreatedAt,
    string? Text,
    string? FileName,
    string? ContentType);
