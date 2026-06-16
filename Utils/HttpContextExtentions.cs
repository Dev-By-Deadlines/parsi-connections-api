namespace Connecions.Api.Utils;

public static class HttpContextExtensions
{
    public static List<string> GetGameSessionIds(this HttpContext httpContext)
    {
        httpContext.Request.Cookies.TryGetValue(GameConstants.SessionCookieName, out var cookieValue);
        return cookieValue?
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .ToList() ?? new();
    }
}
