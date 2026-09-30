using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;
using HotelChecklist.Api.Common.Validation;

namespace HotelChecklist.Api.Features.Calls.Update;

public static class UpdateCallEndpoint
{
    public static void MapUpdateCall(this RouteGroupBuilder group)
    {
        group.MapPut("/{id:guid}", async (
                Guid id,
                UpdateCallRequest request,
                ICommandHandler<UpdateCallCommand, UpdateCallResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(request.ToCommand(id), cancellationToken);
                return result.ToHttpResult();
            })
            .AddEndpointFilter<ValidationFilter<UpdateCallRequest>>()
            .RequireAuthorization(Policies.SupervisorOrAbove)
            .WithName("UpdateCall")
            .Produces<UpdateCallResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
