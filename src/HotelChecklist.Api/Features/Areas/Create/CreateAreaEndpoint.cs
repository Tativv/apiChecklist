using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;
using HotelChecklist.Api.Common.Validation;

namespace HotelChecklist.Api.Features.Areas.Create;

public static class CreateAreaEndpoint
{
    public static void MapCreateArea(this RouteGroupBuilder group)
    {
        group.MapPost("/", async (
                CreateAreaRequest request,
                ICommandHandler<CreateAreaCommand, CreateAreaResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(request.ToCommand(), cancellationToken);
                return result.ToHttpResult(StatusCodes.Status201Created);
            })
            .AddEndpointFilter<ValidationFilter<CreateAreaRequest>>()
            .RequireAuthorization(Policies.SupervisorOrAbove)
            .WithName("CreateArea")
            .Produces<CreateAreaResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
