using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ChecklistInstances.UploadEvidence;

public sealed record UploadEvidenceCommand(
    Guid InstanceId,
    Guid TaskExecutionId,
    Guid UploadedByUserId,
    Stream Content,
    string FileName,
    string ContentType,
    long FileSizeBytes) : ICommand<UploadEvidenceResponse>;
