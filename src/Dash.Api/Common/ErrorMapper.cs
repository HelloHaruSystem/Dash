using Dash.Domain.Common;
using Dash.Domain.Errors;

namespace Dash.Api.Common;

internal static class ErrorMapper
{
    internal static int ToStatusCode(Error error)
    {
        return error.Code switch
        {
            _ when error == UserErrors.InvalidCredentials
                => StatusCodes.Status401Unauthorized,
            _ when error == UserErrors.UsernameAlreadyInUse
                => StatusCodes.Status409Conflict,
            _ when error == UserErrors.EmailAlreadyInUse
                => StatusCodes.Status409Conflict,
            _ when error == UserErrors.AccountIsLocked
                => StatusCodes.Status429TooManyRequests,
            // default to 400 bad request
            _ => StatusCodes.Status400BadRequest
        }
        ;
    }
}
