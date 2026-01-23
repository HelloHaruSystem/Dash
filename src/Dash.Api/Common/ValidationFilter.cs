
namespace Dash.Api.Common;

internal sealed class ValidationFilter<T> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
            EndpointFilterInvocationContext context,
            EndpointFilterDelegate next)
    {
        throw new NotImplementedException();
    }
}
