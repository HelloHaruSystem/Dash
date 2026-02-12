namespace Dash.Api.Common;

internal static class CookieSettings
{
    public const string RefreshTokenCookieName = "refreshToken";

    internal static CookieOptions RefreshTokenCookieOptions(DateTime expires) => new()
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict,
        Path = "/api/auth",
        Expires = expires
    };
}
