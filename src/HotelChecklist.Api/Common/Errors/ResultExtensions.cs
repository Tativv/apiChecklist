using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace HotelChecklist.Api.Common.Errors;

public static class ResultExtensions
{
    public static IResult ToHttpResult(this Result result, int successStatusCode = StatusCodes.Status200OK)
    {
        if (result.IsSuccess)
            return Microsoft.AspNetCore.Http.Results.StatusCode(successStatusCode);

        return ToProblemResult(result.Error);
    }

    public static IResult ToHttpResult(this Result<Unit> result, int successStatusCode = StatusCodes.Status204NoContent)
    {
        if (result.IsSuccess)
            return Microsoft.AspNetCore.Http.Results.StatusCode(successStatusCode);

        return ToProblemResult(result.Error);
    }

    public static IResult ToHttpResult<TValue>(this Result<TValue> result, int successStatusCode = StatusCodes.Status200OK)
    {
        if (result.IsSuccess)
            return Microsoft.AspNetCore.Http.Results.Json(result.Value, statusCode: successStatusCode);

        return ToProblemResult(result.Error);
    }

    private static IResult ToProblemResult(Error error)
    {
        var statusCode = MapStatusCode(error.Type);

        return Microsoft.AspNetCore.Http.Results.Problem(
            title: error.Code,
            detail: error.Message,
            statusCode: statusCode,
            extensions: new Dictionary<string, object?> { ["errorType"] = error.Type.ToString() });
    }

    private static int MapStatusCode(ErrorType type) => type switch
    {
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.Forbidden => StatusCodes.Status403Forbidden,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status500InternalServerError
    };
}
