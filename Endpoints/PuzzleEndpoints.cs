using Microsoft.EntityFrameworkCore;

namespace Connecions.Api.Endpoints;

public static class PuzzleEndpoints
{
    public static void MapPuzzleEndpoints(this WebApplication app)
    {
        const string GetPuzzleEndpointName = "GetPuzzle";

        var group = app.MapGroup("/puzzles");

        // group.MapGet("/", GetPuzzles);
        // group.MapGet("/{id}", GetPuzzle).WithName(GetPuzzleEndpointName);
        // group.MapPost("/", CreatePuzzle);
        //
        // group.MapGet("/daily", GetDailyPuzzle);
    }
}
