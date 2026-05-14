namespace Connecions.Api.Filters;

public class ApiKeyAuthFilter : IEndpointFilter
{
    private readonly string _validKey;
    private const string ApiKeyHeaderName = "X-Api-Key";

    public ApiKeyAuthFilter(string validKey)
    {
        _validKey = validKey;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedKey))
        {
            return Results.Unauthorized(); // 401
        }

        if (!string.Equals(extractedKey, _validKey, StringComparison.Ordinal))
        {
            return Results.Unauthorized();
        }

        return await next(context);
    }
}
