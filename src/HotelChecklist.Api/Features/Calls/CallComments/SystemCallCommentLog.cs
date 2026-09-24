using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Entities;

namespace HotelChecklist.Api.Features.Calls.CallComments;

/// <summary>Registra, como un comentario más, cada cambio de status de un chamado (auditoría visible al usuario).</summary>
public static class SystemCallCommentLog
{
    public static void Add(AppDbContext db, Guid callId, Guid authorUserId, string text, DateTimeOffset at) =>
        db.CallComments.Add(new CallComment
        {
            Id = Guid.NewGuid(),
            CallId = callId,
            Text = text,
            CreatedAt = at,
            AuthorUserId = authorUserId
        });
}
