using HotelChecklist.Api.Common.Adapters.FileStorage;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ServiceOrders.ServiceOrderComments;

public sealed class AddServiceOrderCommentHandler(AppDbContext db, IFileStorage fileStorage)
    : ICommandHandler<AddServiceOrderCommentCommand, ServiceOrderCommentResponseItem>
{
    private static readonly string[] AllowedContentTypes = ["image/jpeg", "image/png", "image/webp", "image/heic"];

    public async Task<Result<ServiceOrderCommentResponseItem>> Handle(AddServiceOrderCommentCommand command, CancellationToken cancellationToken)
    {
        var text = command.Text?.Trim();

        if (string.IsNullOrEmpty(text) && command.FileContent is null)
            return Result.Failure<ServiceOrderCommentResponseItem>(
                Error.Validation("ServiceOrderComments.Empty", "O comentário precisa de um texto ou de um arquivo."));

        if (text is { Length: > 2000 })
            return Result.Failure<ServiceOrderCommentResponseItem>(
                Error.Validation("ServiceOrderComments.TextTooLong", "O comentário não pode ter mais de 2000 caracteres."));

        var serviceOrderExists = await db.ServiceOrders.AnyAsync(so => so.Id == command.ServiceOrderId, cancellationToken);

        if (!serviceOrderExists)
            return Result.Failure<ServiceOrderCommentResponseItem>(Error.NotFound("ServiceOrders.NotFound", "Ordem de serviço no encontrada."));

        string? filePath = null;

        if (command.FileContent is not null)
        {
            if (command.FileContentType is null || !AllowedContentTypes.Contains(command.FileContentType))
                return Result.Failure<ServiceOrderCommentResponseItem>(
                    Error.Validation("ServiceOrderComments.InvalidContentType", "Solo se permiten imágenes (jpeg, png, webp, heic)."));

            filePath = await fileStorage.SaveAsync(command.FileContent, command.FileName ?? "file", command.FileContentType, cancellationToken);
        }

        var comment = new ServiceOrderComment
        {
            Id = Guid.NewGuid(),
            ServiceOrderId = command.ServiceOrderId,
            Text = string.IsNullOrEmpty(text) ? null : text,
            CreatedAt = DateTimeOffset.UtcNow,
            AuthorUserId = command.ActingUserId,
            FilePath = filePath,
            FileName = filePath is null ? null : command.FileName,
            ContentType = filePath is null ? null : command.FileContentType,
            FileSizeBytes = filePath is null ? null : command.FileSizeBytes
        };

        db.ServiceOrderComments.Add(comment);
        await db.SaveChangesAsync(cancellationToken);

        var authorName = await db.Users.Where(u => u.Id == command.ActingUserId).Select(u => u.Name).FirstAsync(cancellationToken);

        return Result.Success(new ServiceOrderCommentResponseItem(
            comment.Id, comment.AuthorUserId, authorName, comment.CreatedAt, comment.Text, comment.FileName, comment.ContentType));
    }
}
