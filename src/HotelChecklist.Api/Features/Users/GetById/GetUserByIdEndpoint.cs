using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.Users.GetById;

public static class GetUserByIdEndpoint
{
    public static void MapGetUserById(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", async (
                Guid id,
                IQueryHandler<GetUserByIdQuery, GetUserByIdResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new GetUserByIdQuery(id), cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.AnyRole)
            .WithName("GetUserById")
            .Produces<GetUserByIdResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
