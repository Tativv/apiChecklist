using System.Security.Claims;
using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;
using HotelChecklist.Api.Common.Validation;

namespace HotelChecklist.Api.Features.ChecklistTemplates.ConfigureAssets;

public static class ConfigureTemplateAssetsEndpoint
{
    public static void MapConfigureTemplateAssets(this RouteGroupBuilder group)
    {
        group.MapPut("/{id:guid}/assets", async (
                Guid id,
                ConfigureTemplateAssetsRequest request,
                ClaimsPrincipal user,
                ICommandHandler<ConfigureTemplateAssetsCommand, ConfigureTemplateAssetsResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(request.ToCommand(id, user.GetRole()), cancellationToken);
                return result.ToHttpResult();
            })
            .AddEndpointFilter<ValidationFilter<ConfigureTemplateAssetsRequest>>()
            .RequireAuthorization(Policies.SupervisorOrAbove)
            .WithName("ConfigureTemplateAssets")
            .Produces<ConfigureTemplateAssetsResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
