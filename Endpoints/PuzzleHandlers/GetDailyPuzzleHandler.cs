using Connecions.Api.Data;
using Connecions.Api.Mapping;
using Connecions.Api.Models;
using Connecions.Api.Utils;
using Microsoft.EntityFrameworkCore;

namespace Connecions.Api.Endpoints.PuzzleHandlers;

public static class GetDailyPuzzle
{
    public static async Task<IResult> Handler(ConnectionsContext dbContext, HttpContext httpContext)
    {
        var cookieName = GameConstants.SessionCookieName;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var dailyPuzzle = await GetDailyPuzzleObject(dbContext, today);
        var puzzle = dailyPuzzle.Puzzle;

        GameState? state = null;
        if (httpContext.Request.Cookies.TryGetValue(cookieName, out var sessionId))
            state = await dbContext.GameStates.FirstOrDefaultAsync(gs => gs.SessionId == sessionId);

        if (state is null)
        {
            sessionId = Guid.NewGuid().ToString();
            state = new GameState
            {
                SessionId = sessionId,
                PuzzleId = puzzle.Id,
                Outcome = Outcomes.Playing,
                RemainingHealth = 4,
                SolvedCategoryIds = ""
            };
            dbContext.GameStates.Add(state);
        }
        else if (state.PuzzleId != puzzle.Id)
        {
            // New day
            state.PuzzleId = puzzle.Id;
            state.RemainingHealth = 4;
            state.Outcome = Outcomes.Playing;
            state.SolvedCategoryIds = "";
        }

        // Already lost or won?
        var solvedCount = state.SolvedCategoryIds.Split(',', StringSplitOptions.RemoveEmptyEntries).Length;
        if (state.RemainingHealth <= 0)
            state.Outcome = Outcomes.Lost;
        else if (solvedCount >= 4)
            state.Outcome = Outcomes.Won;

        await dbContext.SaveChangesAsync();

        // Set cookie if playing
        if (state.Outcome == Outcomes.Playing)
        {
            httpContext.Response.Cookies.Append(cookieName, state.SessionId, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });
        }

        var dto = state.ToDto(puzzle);
        return Results.Ok(dto);
    }

    async static Task<DailyPuzzle> GetDailyPuzzleObject(ConnectionsContext dbContext, DateOnly today)
    {
        DailyPuzzle? dailyPuzzleEntity = await dbContext.DailyPuzzles
            .Include(dp => dp.Puzzle)
            .ThenInclude(p => p.Categories)
            .ThenInclude(c => c.Words)
            .FirstOrDefaultAsync(dp => dp.Date == today);

        if (dailyPuzzleEntity is null)
        {
            var puzzle = await dbContext.Puzzles
                .Include(p => p.Categories)
                .ThenInclude(c => c.Words)
                .OrderBy(p => p.LastUsed)
                .FirstAsync();

            puzzle.LastUsed = today;

            dailyPuzzleEntity = new DailyPuzzle
            {
                Date = today,
                PuzzleId = puzzle.Id,
                Puzzle = puzzle
            };
            dbContext.DailyPuzzles.Add(dailyPuzzleEntity);
        }

        return dailyPuzzleEntity;
    }
}
