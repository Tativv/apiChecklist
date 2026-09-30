using HotelChecklist.Api.Common.Adapters.FileStorage;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Entities;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.Calls.Create;

public sealed class CreateCallHandler(AppDbContext db, IFileStorage fileStorage) : ICommandHandler<CreateCallCommand, CreateCallResponse>
{
    private static readonly string[] AllowedContentTypes = ["image/jpeg", "image/png", "image/webp", "image/heic"];

    public async Task<Result<CreateCallResponse>> Handle(CreateCallCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Subject) || command.Subject.Length > 200)
            return Result.Failure<CreateCallResponse>(
                Error.Validation("Calls.InvalidSubject", "O assunto é obrigatório e deve ter até 200 caracteres."));

        if (command.Description is { Length: > 2000 })
            return Result.Failure<CreateCallResponse>(
                Error.Validation("Calls.DescriptionTooLong", "A descrição não pode ter mais de 2000 caracteres."));

        if (!Enum.TryParse<CallPriority>(command.Priority, ignoreCase: true, out var priority))
            return Result.Failure<CreateCallResponse>(
                Error.Validation("Calls.InvalidPriority", $"Priority must be one of: {string.Join(", ", Enum.GetNames<CallPriority>())}."));

        if (command.FileContent is not null && (command.FileContentType is null || !AllowedContentTypes.Contains(command.FileContentType)))
            return Result.Failure<CreateCallResponse>(
                Error.Validation("Calls.InvalidContentType", "Solo se permiten imágenes (jpeg, png, webp, heic)."));

        var area = await db.Areas.FirstOrDefaultAsync(a => a.Id == command.AreaId, cancellationToken);

        if (area is null)
            return Result.Failure<CreateCallResponse>(Error.NotFound("Areas.NotFound", "Área no encontrada."));

        var createdByUser = await db.Users.FirstAsync(u => u.Id == command.CreatedByUserId, cancellationToken);

        var call = new Call
        {
            Id = Guid.NewGuid(),
            AreaId = command.AreaId,
            Subject = command.Subject,
            Description = command.Description,
            Priority = priority,
            Status = CallStatus.Open,
            CreatedByUserId = command.CreatedByUserId
        };

        db.Calls.Add(call);

        if (command.FileContent is not null)
        {
            var filePath = await fileStorage.SaveAsync(command.FileContent, command.FileName ?? "file", command.FileContentType!, cancellationToken);

            db.CallComments.Add(new CallComment
            {
                Id = Guid.NewGuid(),
                CallId = call.Id,
                Text = null,
                CreatedAt = DateTimeOffset.UtcNow,
                AuthorUserId = command.CreatedByUserId,
                FilePath = filePath,
                FileName = command.FileName,
                ContentType = command.FileContentType,
                FileSizeBytes = command.FileSizeBytes
            });
        }

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(new CreateCallResponse(
            call.Id, call.AreaId, area.Name, call.Subject, call.Description, call.Priority.ToString(), call.Status.ToString(),
            call.CreatedByUserId, createdByUser.Name, call.AssignedUserId, null, call.StartedAt, call.CompletedAt, call.DurationSeconds, call.CreatedAtUtc));
    }
}
