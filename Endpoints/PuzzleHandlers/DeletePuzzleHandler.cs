using Connecions.Api.Data;

namespace Connecions.Api.Endpoints.PuzzleHandlers;

public static class DeletePuzzle
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
