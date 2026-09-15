using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.ChecklistInstances.GetEvidenceFile;

public static class GetEvidenceFileEndpoint
{
    public static void MapGetEvidenceFile(this RouteGroupBuilder group)
    {
        group.MapGet("/evidence/{evidenceId:guid}", async (
                Guid evidenceId,
                IQueryHandler<GetEvidenceFileQuery, GetEvidenceFileResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new GetEvidenceFileQuery(evidenceId), cancellationToken);

                if (result.IsFailure)
                    return result.ToHttpResult();

                return Microsoft.AspNetCore.Http.Results.File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
            })
            .RequireAuthorization(Policies.AnyRole)
            .WithName("GetEvidenceFile")
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
