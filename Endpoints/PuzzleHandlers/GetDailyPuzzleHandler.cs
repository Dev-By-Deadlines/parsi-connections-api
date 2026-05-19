using Connecions.Api.Data;
using Connecions.Api.Dtos;
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

        DailyPuzzle? dailyPuzzleEntity = await dbContext.DailyPuzzle
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
            dbContext.DailyPuzzle.Add(dailyPuzzleEntity);
        }

        // Session management
        GameState? state = null;
        if (httpContext.Request.Cookies.TryGetValue(cookieName, out var sessionId))
            state = await dbContext.GameState.FirstOrDefaultAsync(gs => gs.SessionId == sessionId);

        if (state is null)
        {
            sessionId = Guid.NewGuid().ToString();
            state = new GameState
            {
                SessionId = sessionId,
                PuzzleId = dailyPuzzleEntity.Puzzle.Id,
                RemainingHealth = 4
            };
            dbContext.GameState.Add(state);
        }
        else
        {
            // New day? Reset health and puzzle
            if (state.PuzzleId != dailyPuzzleEntity.Puzzle.Id)
            {
                state.PuzzleId = dailyPuzzleEntity.Puzzle.Id;
                state.RemainingHealth = 4;
                state.SolvedCategoryIds = "";
            }
        }

        await dbContext.SaveChangesAsync();

        // Check for finished game (loss or win)
        var solvedIds = state.SolvedCategoryIds
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .ToHashSet();

        if (state.RemainingHealth <= 0 || solvedIds.Count == 4)
        {
            // Game already over — return the answers
            var allCategories = dailyPuzzleEntity.Puzzle.Categories.Select(c => new
            {
                c.Name,
                Words = c.Words.Select(w => w.Text).ToList()
            }).ToList();

            return Results.Ok(new ResultsDto(allCategories));
        }

        // Active game — set/refresh the cookie
        httpContext.Response.Cookies.Append(cookieName, state.SessionId, new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(2)
        });

        var playerPuzzleDto = dailyPuzzleEntity.Puzzle.ToPlayerPuzzleDto();
        playerPuzzleDto.RemainingHealth = state.RemainingHealth;

        return Results.Ok(playerPuzzleDto);
    }
}
