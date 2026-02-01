using Dash.Domain.Common;
using Dash.Domain.Errors;

namespace Dash.Api.Common;

internal static class ResultExtensions
{
    internal static IResult ToHttpResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return TypedResults.Ok(result.Value);
        }

        int statusCode = MapErrorToStatusCode(result.Error);
        return TypedResults.Json(result.Error, statusCode: statusCode);
    }

    /// <summary>
    /// Converts a result to IResult for POST endpoints that creates resources (401 created)
    /// </summary>
    internal static IResult ToCreatedResult<T>(this Result<T> result, string uri = "")
    {
        if (result.IsSuccess)
        {
            return TypedResults.Created(uri, result.Value);
        }

        int statusCode = MapErrorToStatusCode(result.Error);
        return TypedResults.Json(result.Error, statusCode: statusCode);
    }

    private static int MapErrorToStatusCode(Error error)
    {
        return error.Code switch
        {
            // Authentication errors -> 401 unauthorized
            _ when error == UserErrors.InvalidCredentials
                => StatusCodes.Status401Unauthorized,

            // Conflict errors -> 409 conflict
            _ when error == UserErrors.UsernameAlreadyInUse
                => StatusCodes.Status409Conflict,
            _ when error == UserErrors.EmailAlreadyInUse
                => StatusCodes.Status409Conflict,

            // Default -> 400 BadRequest for other errors
            _ => StatusCodes.Status400BadRequest
        };
    }
}
