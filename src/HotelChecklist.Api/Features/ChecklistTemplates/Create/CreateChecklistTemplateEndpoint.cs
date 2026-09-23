using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;
using HotelChecklist.Api.Common.Validation;

namespace HotelChecklist.Api.Features.ChecklistTemplates.Create;

public static class CreateChecklistTemplateEndpoint
{
    public static void MapCreateChecklistTemplate(this RouteGroupBuilder group)
    {
        group.MapPost("/", async (
                CreateChecklistTemplateRequest request,
                ICommandHandler<CreateChecklistTemplateCommand, CreateChecklistTemplateResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(request.ToCommand(), cancellationToken);
                return result.ToHttpResult(StatusCodes.Status201Created);
            })
            .AddEndpointFilter<ValidationFilter<CreateChecklistTemplateRequest>>()
            .RequireAuthorization(Policies.ManagerOrAbove)
            .WithName("CreateChecklistTemplate")
            .Produces<CreateChecklistTemplateResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
