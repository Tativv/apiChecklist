using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;
using HotelChecklist.Api.Common.Validation;

namespace HotelChecklist.Api.Features.ChecklistTemplates.ApplyToAssets;

public static class ApplyTemplateToAssetsEndpoint
{
    public static void MapApplyTemplateToAssets(this RouteGroupBuilder group)
    {
        group.MapPost("/{id:guid}/apply-to-assets", async (
                Guid id,
                ApplyTemplateToAssetsRequest request,
                ICommandHandler<ApplyTemplateToAssetsCommand, ApplyTemplateToAssetsResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(request.ToCommand(id), cancellationToken);
                return result.ToHttpResult(StatusCodes.Status201Created);
            })
            .AddEndpointFilter<ValidationFilter<ApplyTemplateToAssetsRequest>>()
            .RequireAuthorization(Policies.SupervisorOrAbove)
            .WithName("ApplyTemplateToAssets")
            .Produces<ApplyTemplateToAssetsResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
