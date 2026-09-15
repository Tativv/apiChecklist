namespace HotelChecklist.Api.Features.ChecklistInstances.UploadEvidence;

public sealed record UploadEvidenceResponse(Guid Id, string FileName, string ContentType, long FileSizeBytes, DateTimeOffset UploadedAt);
