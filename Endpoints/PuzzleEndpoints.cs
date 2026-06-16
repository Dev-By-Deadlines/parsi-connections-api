using Connecions.Api.Endpoints.PuzzleHandlers;
using Connecions.Api.Filters;

namespace Connecions.Api.Endpoints;

public static class PuzzleEndpoints
{
    public const string GetPuzzleEndpointName = "GetPuzzle";

    public static void MapPuzzleEndpoints(this WebApplication app, string adminKey)
    {
        var group = app.MapGroup("/puzzles");
        var adminFilter = new ApiKeyAuthFilter(adminKey);

        // ------------Player----------------------------------------------------------
        group.MapGet("/daily", GetDailyPuzzle.Handler);
        group.MapPost("/{id}/guess", Guess.Handler);
        group.MapGet("/{id}/stats", GetStats.Handler);
        group.MapGet("/archive", GetArchive.Handler);
        group.MapGet("/{id}/play", GetPuzzleState.Handler);

        // ------------Admin-----------------------------------------------------------
        group.MapGet("/", GetPuzzles.Handler)
            .AddEndpointFilter(adminFilter);

        group.MapGet("/{id}", GetPuzzle.Handler).WithName(GetPuzzleEndpointName)
            .AddEndpointFilter(adminFilter);

        group.MapPost("/", CreatePuzzle.Handler)
            .AddEndpointFilter(adminFilter);

        group.MapDelete("/{id}", DeletePuzzle.Handler)
            .AddEndpointFilter(adminFilter);

        group.MapPut("/{id}", UpdatePuzzle.Handler)
            .AddEndpointFilter(adminFilter);
    }
}
