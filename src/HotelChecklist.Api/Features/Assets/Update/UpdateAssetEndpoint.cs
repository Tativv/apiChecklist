using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;
using HotelChecklist.Api.Common.Validation;

namespace HotelChecklist.Api.Features.Assets.Update;

public static class UpdateAssetEndpoint
{
    public static void MapUpdateAsset(this RouteGroupBuilder group)
    {
        group.MapPut("/{id:guid}", async (
                Guid id,
                UpdateAssetRequest request,
                ICommandHandler<UpdateAssetCommand, UpdateAssetResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(request.ToCommand(id), cancellationToken);
                return result.ToHttpResult();
            })
            .AddEndpointFilter<ValidationFilter<UpdateAssetRequest>>()
            .RequireAuthorization(Policies.SupervisorOrAbove)
            .WithName("UpdateAsset")
            .Produces<UpdateAssetResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
