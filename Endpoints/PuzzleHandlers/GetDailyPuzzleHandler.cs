using Connecions.Api.Mapping;
using Connecions.Api.Services;
using Connecions.Api.Utils;

namespace Connecions.Api.Endpoints.PuzzleHandlers;

public class GetDailyPuzzle
{
    public static async Task<IResult> Handler(
        HttpContext httpContext,
        PuzzleService puzzleService,
        GameStateService gameStateService,
        ILogger<GetDailyPuzzle> logger)
    {
        var cookieName = GameConstants.SessionCookieName;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var dailyPuzzle = await puzzleService.GetOrCreateDailyPuzzleAsync(today);
        var puzzle = dailyPuzzle.Puzzle;

        httpContext.Request.Cookies.TryGetValue(cookieName, out var cookieValue);
        var sessionIds = cookieValue?
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .ToList() ?? new List<string>();

        var state = await gameStateService.GetOrCreateForPuzzleAsync(sessionIds, puzzle);

        // If this is a new session, add it to the list
        if (!sessionIds.Contains(state.SessionId))
            sessionIds.Add(state.SessionId);

        await gameStateService.SaveAsync();

        httpContext.Response.Cookies.Append(cookieName, string.Join(',', sessionIds), new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(400)
        });

        var dto = state.ToDto(puzzle);
        return Results.Ok(dto);
    }
}
