using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;

namespace HotelChecklist.Api.Features.Areas.GetById;

public sealed class GetAreaByIdHandler(AppDbContext db) : IQueryHandler<GetAreaByIdQuery, GetAreaByIdResponse>
{
    public async Task<Result<GetAreaByIdResponse>> Handle(GetAreaByIdQuery query, CancellationToken cancellationToken)
    {
        var area = await db.Areas.FindAsync([query.Id], cancellationToken);

        if (area is null)
            return Result.Failure<GetAreaByIdResponse>(Error.NotFound("Areas.NotFound", "Área no encontrada."));

        return Result.Success(area.ToResponse());
    }
}
