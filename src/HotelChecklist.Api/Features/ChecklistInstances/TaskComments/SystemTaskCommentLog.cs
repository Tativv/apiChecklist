using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Entities;

namespace HotelChecklist.Api.Features.ChecklistInstances.TaskComments;

/// <summary>Registra, como un comentario más, cada cambio de status de una tarea (auditoría visible al usuario).</summary>
public static class SystemTaskCommentLog
{
    public static void Add(AppDbContext db, Guid taskExecutionId, Guid authorUserId, string text, DateTimeOffset at) =>
        db.ChecklistTaskComments.Add(new ChecklistTaskComment
        {
            Id = Guid.NewGuid(),
            ChecklistTaskExecutionId = taskExecutionId,
            Text = text,
            CreatedAt = at,
            AuthorUserId = authorUserId
        });
}
