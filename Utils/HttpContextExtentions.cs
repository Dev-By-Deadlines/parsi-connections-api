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

    public static void AppendCookieWithNewValue(this HttpContext httpContext, List<string> sessionIds)
    {
        httpContext.Response.Cookies.Append(GameConstants.SessionCookieName, string.Join(',', sessionIds), new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(400)
        });
    }
}
