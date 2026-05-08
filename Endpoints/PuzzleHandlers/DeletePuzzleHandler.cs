using Connecions.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Connecions.Api.Endpoints.PuzzleHandlers;

public static class DeletePuzzleHandler
{
    public static async Task<IResult> Handler(int id, ConnectionsContext dbContext)
    {
        var puzzle = await dbContext.Puzzles.FindAsync(id);
        if (puzzle is null) return Results.NotFound();

        dbContext.Remove(puzzle);
        await dbContext.SaveChangesAsync();

        return Results.NoContent();
    }
}
