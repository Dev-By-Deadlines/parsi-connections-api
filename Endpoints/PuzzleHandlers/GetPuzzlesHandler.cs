using Connecions.Api.Data;
using Connecions.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Connecions.Api.Endpoints.PuzzleHandlers;

public static class GetPuzzles
{
    public async static Task<IResult> Handler(ConnectionsContext dbContext)
    {
        var puzzles = await dbContext.Puzzles
            .Include(p => p.Categories)
            .ThenInclude(c => c.Words)
            .ToListAsync();

        var dtos = puzzles.Select(p => p.ToAdminPuzzleDto()).ToList();

        return Results.Ok(dtos);
    }
}
