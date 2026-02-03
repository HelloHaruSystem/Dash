namespace Dash.Api.Common;

internal static class RateLimitPolicies
{
    /// <summary>
    /// Default policy for API endpoints 100 requests per minute
    /// </summary>
    public const string Api = "api";

    /// <summary>
    /// Default policy for API endpoints 10 requests per minute
    /// </summary>
    public const string Login = "login";

    /// <summary>
    /// Default policy for API endpoints 5 requests per minute
    /// </summary>
    public const string Register = "register";
}
