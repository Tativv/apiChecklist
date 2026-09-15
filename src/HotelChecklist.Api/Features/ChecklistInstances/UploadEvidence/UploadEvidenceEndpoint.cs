using System.Security.Claims;
using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.ChecklistInstances.UploadEvidence;

public static class UploadEvidenceEndpoint
{
    public static void MapUploadEvidence(this RouteGroupBuilder group)
    {
        group.MapPost("/{instanceId:guid}/tasks/{taskExecutionId:guid}/evidence", async (
                Guid instanceId,
                Guid taskExecutionId,
                IFormFile file,
                ClaimsPrincipal user,
                ICommandHandler<UploadEvidenceCommand, UploadEvidenceResponse> handler,
                CancellationToken cancellationToken) =>
            {
                await using var stream = file.OpenReadStream();

                var command = new UploadEvidenceCommand(
                    instanceId, taskExecutionId, user.GetUserId(), stream, file.FileName, file.ContentType, file.Length);

                var result = await handler.Handle(command, cancellationToken);
                return result.ToHttpResult(StatusCodes.Status201Created);
            })
            .DisableAntiforgery()
            .RequireAuthorization(Policies.AnyRole)
            .WithName("UploadEvidence")
            .Produces<UploadEvidenceResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
