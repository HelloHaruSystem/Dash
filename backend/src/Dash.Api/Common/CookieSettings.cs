namespace Dash.Api.Common;

internal static class CookieSettings
{
    internal static CookieOptions RefreshTokenCookieOptions(DateTime expires) => new()
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict,
        Path = "/api/auth",
        Expires = expires
    };
}
