using Connecions.Api.Mapping;
using Connecions.Api.Services;
using Connecions.Api.Utils;

namespace Connecions.Api.Endpoints.PuzzleHandlers;

public class GetPuzzleState
{
    public static async Task<IResult> Handler(
        int id,
        HttpContext httpContext,
        PuzzleService puzzleService,
        GameStateService gameStateService)
    {
        var puzzle = await puzzleService.GetArchivedPuzzleWithCategoriesAsync(id);

        if (puzzle is null)
            return Results.NotFound("Puzzle not found.");

        var sessionIds = httpContext.GetGameSessionIds();

        var state = await gameStateService.GetOrCreateForPuzzleAsync(sessionIds, puzzle);

        if (!sessionIds.Contains(state.SessionId))
            sessionIds.Add(state.SessionId);

        await gameStateService.SaveAsync();

        httpContext.AppendCookieWithNewValue(sessionIds);

        var dto = state.ToDto(puzzle);
        return Results.Ok(dto);
    }
}
