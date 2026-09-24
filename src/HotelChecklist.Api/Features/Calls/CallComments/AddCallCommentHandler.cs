using HotelChecklist.Api.Common.Adapters.FileStorage;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.Calls.CallComments;

public sealed class AddCallCommentHandler(AppDbContext db, IFileStorage fileStorage) : ICommandHandler<AddCallCommentCommand, CallCommentResponseItem>
{
    private static readonly string[] AllowedContentTypes = ["image/jpeg", "image/png", "image/webp", "image/heic"];

    public async Task<Result<CallCommentResponseItem>> Handle(AddCallCommentCommand command, CancellationToken cancellationToken)
    {
        var text = command.Text?.Trim();

        if (string.IsNullOrEmpty(text) && command.FileContent is null)
            return Result.Failure<CallCommentResponseItem>(
                Error.Validation("CallComments.Empty", "O comentário precisa de um texto ou de um arquivo."));

        if (text is { Length: > 2000 })
            return Result.Failure<CallCommentResponseItem>(
                Error.Validation("CallComments.TextTooLong", "O comentário não pode ter mais de 2000 caracteres."));

        var callExists = await db.Calls.AnyAsync(c => c.Id == command.CallId, cancellationToken);

        if (!callExists)
            return Result.Failure<CallCommentResponseItem>(Error.NotFound("Calls.NotFound", "Chamado no encontrado."));

        string? filePath = null;

        if (command.FileContent is not null)
        {
            if (command.FileContentType is null || !AllowedContentTypes.Contains(command.FileContentType))
                return Result.Failure<CallCommentResponseItem>(
                    Error.Validation("CallComments.InvalidContentType", "Solo se permiten imágenes (jpeg, png, webp, heic)."));

            filePath = await fileStorage.SaveAsync(command.FileContent, command.FileName ?? "file", command.FileContentType, cancellationToken);
        }

        var comment = new CallComment
        {
            Id = Guid.NewGuid(),
            CallId = command.CallId,
            Text = string.IsNullOrEmpty(text) ? null : text,
            CreatedAt = DateTimeOffset.UtcNow,
            AuthorUserId = command.ActingUserId,
            FilePath = filePath,
            FileName = filePath is null ? null : command.FileName,
            ContentType = filePath is null ? null : command.FileContentType,
            FileSizeBytes = filePath is null ? null : command.FileSizeBytes
        };

        db.CallComments.Add(comment);
        await db.SaveChangesAsync(cancellationToken);

        var authorName = await db.Users.Where(u => u.Id == command.ActingUserId).Select(u => u.Name).FirstAsync(cancellationToken);

        return Result.Success(new CallCommentResponseItem(
            comment.Id, comment.AuthorUserId, authorName, comment.CreatedAt, comment.Text, comment.FileName, comment.ContentType));
    }
}
