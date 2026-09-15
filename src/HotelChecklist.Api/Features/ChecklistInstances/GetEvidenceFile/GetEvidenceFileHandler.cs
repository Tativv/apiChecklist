using HotelChecklist.Api.Common.Adapters.FileStorage;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ChecklistInstances.GetEvidenceFile;

public sealed class GetEvidenceFileHandler(AppDbContext db, IFileStorage fileStorage) : IQueryHandler<GetEvidenceFileQuery, GetEvidenceFileResponse>
{
    public async Task<Result<GetEvidenceFileResponse>> Handle(GetEvidenceFileQuery query, CancellationToken cancellationToken)
    {
        var evidence = await db.ChecklistTaskEvidences.FindAsync([query.EvidenceId], cancellationToken);

        if (evidence is null)
            return Result.Failure<GetEvidenceFileResponse>(Error.NotFound("ChecklistTaskEvidences.NotFound", "Evidencia no encontrada."));

        var stream = await fileStorage.ReadAsync(evidence.FilePath, cancellationToken);

        return Result.Success(new GetEvidenceFileResponse(stream, evidence.ContentType, evidence.FileName));
    }
}
