using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.Calls.CallComments;

public sealed record AddCallCommentCommand(
    Guid CallId,
    string? Text,
    Guid ActingUserId,
    Stream? FileContent,
    string? FileName,
    string? FileContentType,
    long? FileSizeBytes) : ICommand<CallCommentResponseItem>;
