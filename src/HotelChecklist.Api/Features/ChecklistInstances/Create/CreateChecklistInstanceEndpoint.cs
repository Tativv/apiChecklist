using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;
using HotelChecklist.Api.Common.Validation;

namespace HotelChecklist.Api.Features.ChecklistInstances.Create;

public static class CreateChecklistInstanceEndpoint
{
    public static void MapCreateChecklistInstance(this RouteGroupBuilder group)
    {
        group.MapPost("/", async (
                CreateChecklistInstanceRequest request,
                ICommandHandler<CreateChecklistInstanceCommand, CreateChecklistInstanceResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(request.ToCommand(), cancellationToken);
                return result.ToHttpResult(StatusCodes.Status201Created);
            })
            .AddEndpointFilter<ValidationFilter<CreateChecklistInstanceRequest>>()
            .RequireAuthorization(Policies.ManagerOrAbove)
            .WithName("CreateChecklistInstance")
            .Produces<CreateChecklistInstanceResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
