using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;
using HotelChecklist.Api.Common.Validation;

namespace HotelChecklist.Api.Features.Assets.Create;

public static class CreateAssetEndpoint
{
    public static void MapCreateAsset(this RouteGroupBuilder group)
    {
        group.MapPost("/", async (
                CreateAssetRequest request,
                ICommandHandler<CreateAssetCommand, CreateAssetResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(request.ToCommand(), cancellationToken);
                return result.ToHttpResult(StatusCodes.Status201Created);
            })
            .AddEndpointFilter<ValidationFilter<CreateAssetRequest>>()
            .RequireAuthorization(Policies.SupervisorOrAbove)
            .WithName("CreateAsset")
            .Produces<CreateAssetResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
