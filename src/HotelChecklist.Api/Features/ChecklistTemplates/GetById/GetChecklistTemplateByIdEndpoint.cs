using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.ChecklistTemplates.GetById;

public static class GetChecklistTemplateByIdEndpoint
{
    public static void MapGetChecklistTemplateById(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", async (
                Guid id,
                IQueryHandler<GetChecklistTemplateByIdQuery, GetChecklistTemplateByIdResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new GetChecklistTemplateByIdQuery(id), cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.AnyRole)
            .WithName("GetChecklistTemplateById")
            .Produces<GetChecklistTemplateByIdResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
