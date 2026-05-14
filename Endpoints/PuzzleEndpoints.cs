using Connecions.Api.Endpoints.PuzzleHandlers;
using Connecions.Api.Filters;

namespace Connecions.Api.Endpoints;

public static class PuzzleEndpoints
{
    public const string GetPuzzleEndpointName = "GetPuzzle";

    public static void MapPuzzleEndpoints(this WebApplication app, string amdinKey)
    {
        var group = app.MapGroup("/puzzles");
        var adminFilter = new ApiKeyAuthFilter(amdinKey);

        // ------------Admin-----------------------------------------------------------
        group.MapGet("/", GetPuzzlesHandler.Handler)
            .AddEndpointFilter(adminFilter);

        group.MapGet("/{id}", GetPuzzleHandler.Handler).WithName(GetPuzzleEndpointName)
            .AddEndpointFilter(adminFilter);

        group.MapPost("/", CreatePuzzleHandler.Handler)
            .AddEndpointFilter(adminFilter);

        group.MapDelete("/{id}", DeletePuzzleHandler.Handler)
            .AddEndpointFilter(adminFilter);

        // ------------Player-----------------------------------------------------------
        group.MapGet("/daily", GetDailyPuzzleHandler.Handler);
        group.MapPost("{id}/guess", GuessHandler.Handler);
    }
}
