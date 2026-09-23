using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;
using HotelChecklist.Api.Common.Validation;

namespace HotelChecklist.Api.Features.ChecklistTemplates.Update;

public static class UpdateChecklistTemplateEndpoint
{
    public static void MapUpdateChecklistTemplate(this RouteGroupBuilder group)
    {
        group.MapPut("/{id:guid}", async (
                Guid id,
                UpdateChecklistTemplateRequest request,
                ICommandHandler<UpdateChecklistTemplateCommand, UpdateChecklistTemplateResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(request.ToCommand(id), cancellationToken);
                return result.ToHttpResult();
            })
            .AddEndpointFilter<ValidationFilter<UpdateChecklistTemplateRequest>>()
            .RequireAuthorization(Policies.SupervisorOrAbove)
            .WithName("UpdateChecklistTemplate")
            .Produces<UpdateChecklistTemplateResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
