using Connecions.Api.Data;
using Connecions.Api.Mapping;
using Connecions.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Connecions.Api.Endpoints.PuzzleHandlers;

public static class GetDailyPuzzleHandler
{
    public static async Task<IResult> Handler(ConnectionsContext dbContext)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var dailyPuzzle = await dbContext.DailyPuzzle
            .Include(dp => dp.Puzzle)
            .ThenInclude(p => p.Categories)
            .ThenInclude(c => c.Words)
            .FirstOrDefaultAsync(p => p.Date == today);

        if (dailyPuzzle is not null)
        {
            return Results.Ok(dailyPuzzle.Puzzle.ToPlayerPuzzleDto());
        }

        var newPuzzle = await dbContext.Puzzles
            .Include(p => p.Categories)
            .ThenInclude(c => c.Words)
            .OrderBy(s => s.LastUsed)
            .FirstAsync();

        newPuzzle.LastUsed = today;

        await dbContext.DailyPuzzle.AddAsync(new DailyPuzzle()
        {
            PuzzleId = newPuzzle.Id,
            Puzzle = newPuzzle,
            Date = today
        });

        await dbContext.SaveChangesAsync();

        return Results.Ok(newPuzzle.ToPlayerPuzzleDto());
    }
}
