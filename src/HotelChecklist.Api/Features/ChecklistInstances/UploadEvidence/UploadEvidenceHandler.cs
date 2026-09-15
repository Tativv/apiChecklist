using HotelChecklist.Api.Common.Adapters.FileStorage;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Entities;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ChecklistInstances.UploadEvidence;

public sealed class UploadEvidenceHandler(AppDbContext db, IFileStorage fileStorage) : ICommandHandler<UploadEvidenceCommand, UploadEvidenceResponse>
{
    private static readonly string[] AllowedContentTypes = ["image/jpeg", "image/png", "image/webp", "image/heic"];

    public async Task<Result<UploadEvidenceResponse>> Handle(UploadEvidenceCommand command, CancellationToken cancellationToken)
    {
        var instance = await db.ChecklistInstances.FindAsync([command.InstanceId], cancellationToken);

        if (instance is null)
            return Result.Failure<UploadEvidenceResponse>(Error.NotFound("ChecklistInstances.NotFound", "Checklist no encontrado."));

        if (instance.Status != ChecklistStatus.InProgress)
            return Result.Failure<UploadEvidenceResponse>(
                Error.Conflict("ChecklistInstances.InvalidTransition", "Solo se pueden subir evidencias en un checklist en progreso."));

        var taskExecutionExists = await db.ChecklistTaskExecutions
            .AnyAsync(e => e.Id == command.TaskExecutionId && e.ChecklistInstanceId == command.InstanceId, cancellationToken);

        if (!taskExecutionExists)
            return Result.Failure<UploadEvidenceResponse>(Error.NotFound("ChecklistTaskExecutions.NotFound", "Tarea no encontrada."));

        if (!AllowedContentTypes.Contains(command.ContentType))
            return Result.Failure<UploadEvidenceResponse>(
                Error.Validation("ChecklistTaskEvidences.InvalidContentType", "Solo se permiten imágenes (jpeg, png, webp, heic)."));

        var filePath = await fileStorage.SaveAsync(command.Content, command.FileName, command.ContentType, cancellationToken);

        var evidence = new ChecklistTaskEvidence
        {
            Id = Guid.NewGuid(),
            ChecklistTaskExecutionId = command.TaskExecutionId,
            FilePath = filePath,
            FileName = command.FileName,
            ContentType = command.ContentType,
            FileSizeBytes = command.FileSizeBytes,
            UploadedAt = DateTimeOffset.UtcNow,
            UploadedByUserId = command.UploadedByUserId
        };

        db.ChecklistTaskEvidences.Add(evidence);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(new UploadEvidenceResponse(evidence.Id, evidence.FileName, evidence.ContentType, evidence.FileSizeBytes, evidence.UploadedAt));
    }
}
