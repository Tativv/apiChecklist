using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.Calls.GetById;

public sealed class GetCallByIdHandler(AppDbContext db) : IQueryHandler<GetCallByIdQuery, GetCallByIdResponse>
{
    public async Task<Result<GetCallByIdResponse>> Handle(GetCallByIdQuery query, CancellationToken cancellationToken)
    {
        var response = await db.Calls
            .Where(c => c.Id == query.Id)
            .Select(c => new GetCallByIdResponse(
                c.Id, c.AreaId, c.Area.Name, c.Subject, c.Description, c.Priority.ToString(), c.Status.ToString(),
                c.CreatedByUserId, c.CreatedByUser.Name, c.AssignedUserId, c.AssignedUser != null ? c.AssignedUser.Name : null,
                c.StartedAt, c.CompletedAt, c.DurationSeconds, c.CreatedAtUtc))
            .FirstOrDefaultAsync(cancellationToken);

        if (response is null)
            return Result.Failure<GetCallByIdResponse>(Error.NotFound("Calls.NotFound", "Chamado no encontrado."));

        return Result.Success(response);
    }
}
