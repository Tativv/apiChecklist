using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;
using HotelChecklist.Api.Common.Validation;

namespace HotelChecklist.Api.Features.Users.Update;

public static class UpdateUserEndpoint
{
    public static void MapUpdateUser(this RouteGroupBuilder group)
    {
        group.MapPut("/{id:guid}", async (
                Guid id,
                UpdateUserRequest request,
                ICommandHandler<UpdateUserCommand, UpdateUserResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(request.ToCommand(id), cancellationToken);
                return result.ToHttpResult();
            })
            .AddEndpointFilter<ValidationFilter<UpdateUserRequest>>()
            .RequireAuthorization(Policies.AdminOnly)
            .WithName("UpdateUser")
            .Produces<UpdateUserResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
