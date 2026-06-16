using Connecions.Api.Mapping;
using Connecions.Api.Services;
using Connecions.Api.Utils;

namespace Connecions.Api.Endpoints.PuzzleHandlers;

public class GetDailyPuzzle
{
    public static async Task<IResult> Handler(
        HttpContext httpContext,
        PuzzleService puzzleService,
        GameStateService gameStateService)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var dailyPuzzle = await puzzleService.GetOrCreateDailyPuzzleAsync(today);
        var puzzle = dailyPuzzle.Puzzle;

        var sessionIds = httpContext.GetGameSessionIds();

        var state = await gameStateService.GetOrCreateForPuzzleAsync(sessionIds, puzzle);

        // If this is a new session, add it to the list
        if (!sessionIds.Contains(state.SessionId))
            sessionIds.Add(state.SessionId);

        await gameStateService.SaveAsync();

        httpContext.AppendCookieWithNewValue(sessionIds);

        var dto = state.ToDto(puzzle);
        return Results.Ok(dto);
    }
}
