using Connecions.Api.Data;
using Connecions.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Connecions.Api.Endpoints.PuzzleHandlers;

public static class GetPuzzle
{
    public static async Task<IResult> Handler(int id, ConnectionsContext dbContext)
    {
        var puzzle = await dbContext.Puzzles
            .Include(p => p.Categories)
            .ThenInclude(c => c.Words)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (puzzle is null) return Results.NotFound();

        return Results.Ok(puzzle.ToAdminPuzzleDto());
    }
}
