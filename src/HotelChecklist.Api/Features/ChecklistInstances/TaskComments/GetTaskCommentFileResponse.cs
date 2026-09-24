namespace HotelChecklist.Api.Features.ChecklistInstances.TaskComments;

public sealed record GetTaskCommentFileResponse(Stream Content, string ContentType, string FileName);
