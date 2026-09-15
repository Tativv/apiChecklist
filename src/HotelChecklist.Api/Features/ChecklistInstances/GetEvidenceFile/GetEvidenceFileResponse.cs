namespace HotelChecklist.Api.Features.ChecklistInstances.GetEvidenceFile;

public sealed record GetEvidenceFileResponse(Stream Content, string ContentType, string FileName);
