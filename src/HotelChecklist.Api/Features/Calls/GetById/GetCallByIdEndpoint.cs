using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.Calls.GetById;

public static class GetCallByIdEndpoint
{
    public static void MapGetCallById(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", async (
                Guid id,
                IQueryHandler<GetCallByIdQuery, GetCallByIdResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new GetCallByIdQuery(id), cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.AnyRole)
            .WithName("GetCallById")
            .Produces<GetCallByIdResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
