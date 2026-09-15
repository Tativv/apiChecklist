using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ChecklistInstances.GetEvidenceFile;

public sealed record GetEvidenceFileQuery(Guid EvidenceId) : IQuery<GetEvidenceFileResponse>;
