using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.ChecklistInstances.GetById;

public static class GetChecklistInstanceByIdEndpoint
{
    public static void MapGetChecklistInstanceById(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", async (
                Guid id,
                IQueryHandler<GetChecklistInstanceByIdQuery, GetChecklistInstanceByIdResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new GetChecklistInstanceByIdQuery(id), cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.AnyRole)
            .WithName("GetChecklistInstanceById")
            .Produces<GetChecklistInstanceByIdResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
