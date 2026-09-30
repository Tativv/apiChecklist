using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Entities;

namespace HotelChecklist.Api.Features.ServiceOrders.ServiceOrderComments;

/// <summary>Registra, como un comentario más, cada cambio de status de una orden de servicio (auditoría visible al usuario).</summary>
public static class SystemServiceOrderCommentLog
{
    public static void Add(AppDbContext db, Guid serviceOrderId, Guid authorUserId, string text, DateTimeOffset at) =>
        db.ServiceOrderComments.Add(new ServiceOrderComment
        {
            Id = Guid.NewGuid(),
            ServiceOrderId = serviceOrderId,
            Text = text,
            CreatedAt = at,
            AuthorUserId = authorUserId
        });
}
