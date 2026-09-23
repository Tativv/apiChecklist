using System.Security.Claims;
using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;
using HotelChecklist.Api.Common.Validation;

namespace HotelChecklist.Api.Features.Calls.Create;

public static class CreateCallEndpoint
{
    public static void MapCreateCall(this RouteGroupBuilder group)
    {
        group.MapPost("/", async (
                CreateCallRequest request,
                ClaimsPrincipal user,
                ICommandHandler<CreateCallCommand, CreateCallResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(request.ToCommand(user.GetUserId()), cancellationToken);
                return result.ToHttpResult(StatusCodes.Status201Created);
            })
            .AddEndpointFilter<ValidationFilter<CreateCallRequest>>()
            .RequireAuthorization(Policies.SupervisorOrAbove)
            .WithName("CreateCall")
            .Produces<CreateCallResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
