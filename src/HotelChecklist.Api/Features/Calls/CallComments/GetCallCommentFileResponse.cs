namespace HotelChecklist.Api.Features.Calls.CallComments;

public sealed record GetCallCommentFileResponse(Stream Content, string ContentType, string FileName);
