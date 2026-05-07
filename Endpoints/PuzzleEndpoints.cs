using Microsoft.EntityFrameworkCore;
using Connecions.Api.Mapping;
using Connecions.Api.Data;
using Connecions.Api.Dtos;

namespace Connecions.Api.Endpoints;

public static class PuzzleEndpoints
{
    public static void MapPuzzleEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/puzzle");

        group.MapGet("/", AdminGetPuzzles);

        async Task<IResult> AdminGetPuzzles(ConnectionsContext dbContext)
        {
            var puzzles = await dbContext.Puzzles.ToListAsync();
            var dtos = puzzles.Select(p => p.ToAdminPuzzleDto()).ToList();
            return Results.Ok(dtos);
        }
    }
}
