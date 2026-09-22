using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.Users.GetById;

public sealed class GetUserByIdHandler(AppDbContext db) : IQueryHandler<GetUserByIdQuery, GetUserByIdResponse>
{
    public async Task<Result<GetUserByIdResponse>> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        var user = await db.Users.Include(u => u.UserAreas).FirstOrDefaultAsync(u => u.Id == query.Id, cancellationToken);

        if (user is null)
            return Result.Failure<GetUserByIdResponse>(Error.NotFound("Users.NotFound", "Usuario no encontrado."));

        return Result.Success(user.ToResponse());
    }
}
