using Connecions.Api.Endpoints.PuzzleHandlers;
using Microsoft.EntityFrameworkCore;

namespace Connecions.Api.Endpoints;

public static class PuzzleEndpoints
{
    public const string GetPuzzleEndpointName = "GetPuzzle";

    public static void MapPuzzleEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/puzzles");

        // ------------Admin-----------------------------------------------------------
        group.MapGet("/", GetPuzzlesHandler.Handler);
        group.MapGet("/{id}", GetPuzzleHandler.Handler).WithName(GetPuzzleEndpointName);
        group.MapPost("/", CreatePuzzleHandler.Handler);
        group.MapDelete("/{id}", DeletePuzzleHandler.Handler);

        // ------------Player-----------------------------------------------------------
        group.MapGet("/daily", GetDailyPuzzleHandler.Handler);
    }
}
