using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;
using HotelChecklist.Api.Common.Validation;

namespace HotelChecklist.Api.Features.Users.Create;

public static class CreateUserEndpoint
{
    public static void MapCreateUser(this RouteGroupBuilder group)
    {
        group.MapPost("/", async (
                CreateUserRequest request,
                ICommandHandler<CreateUserCommand, CreateUserResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(request.ToCommand(), cancellationToken);
                return result.ToHttpResult(StatusCodes.Status201Created);
            })
            .AddEndpointFilter<ValidationFilter<CreateUserRequest>>()
            .RequireAuthorization(Policies.AdminOnly)
            .WithName("CreateUser")
            .Produces<CreateUserResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
