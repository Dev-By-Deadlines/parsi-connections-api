using Connecions.Api.Data;
using Connecions.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Connecions.Api.Services;

public class PuzzleService(ConnectionsContext dbContext)
{
    public async Task<DailyPuzzle> GetOrCreateDailyPuzzleAsync(DateOnly date)
    {
        var dailyPuzzle = await dbContext.DailyPuzzles
            .Include(dp => dp.Puzzle)
            .ThenInclude(p => p.Categories)
            .ThenInclude(c => c.Words)
            .FirstOrDefaultAsync(dp => dp.Date == date);

        if (dailyPuzzle is not null)
            return dailyPuzzle;

        var puzzle = await dbContext.Puzzles
            .Include(p => p.Categories)
            .ThenInclude(c => c.Words)
            .OrderBy(p => p.LastUsed)
            .FirstAsync();

        puzzle.LastUsed = date;
        dailyPuzzle = new DailyPuzzle { Date = date, PuzzleId = puzzle.Id, Puzzle = puzzle };
        dbContext.DailyPuzzles.Add(dailyPuzzle);

        return dailyPuzzle;
    }

    public async Task<Puzzle?> GetPuzzleWithCategoriesAsync(int puzzleId) =>
        await dbContext.Puzzles
            .Include(p => p.Categories)
            .ThenInclude(c => c.Words)
            .FirstOrDefaultAsync(p => p.Id == puzzleId);
}
