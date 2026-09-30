using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ServiceOrders.ServiceOrderComments;

public sealed record AddServiceOrderCommentCommand(
    Guid ServiceOrderId,
    string? Text,
    Guid ActingUserId,
    Stream? FileContent,
    string? FileName,
    string? FileContentType,
    long? FileSizeBytes) : ICommand<ServiceOrderCommentResponseItem>;
