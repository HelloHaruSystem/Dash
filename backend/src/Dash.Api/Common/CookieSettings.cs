namespace Dash.Api.Common;

internal static class CookieSettings
{
    public const string RefreshTokenCookieName = "refreshToken";

    internal static CookieOptions RefreshTokenCookieOptions(DateTime expires) => new()
    {
        HttpOnly = true,
        Secure = true,
        // TODO: Set SameSite To Strict
        SameSite = SameSiteMode.None,
        Path = "/api/auth",
        Expires = expires
    };
}
