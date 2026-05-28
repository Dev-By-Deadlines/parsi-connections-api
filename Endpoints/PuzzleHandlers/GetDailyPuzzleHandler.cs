using Connecions.Api.Mapping;
using Connecions.Api.Models;
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

        httpContext.Request.Cookies.TryGetValue(cookieName, out var sessionId);
        sessionId ??= Guid.NewGuid().ToString();

        logger.LogInformation("Daily puzzle requested for session {SessionId}", sessionId);

        var state = await gameStateService.GetOrCreateAsync(sessionId, puzzle);
        await gameStateService.SaveAsync();

        if (state.Outcome == Outcomes.Playing)
        {
            httpContext.Response.Cookies.Append(cookieName, state.SessionId, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });
        }

        return Results.Ok(state.ToDto(puzzle));
    }
}
