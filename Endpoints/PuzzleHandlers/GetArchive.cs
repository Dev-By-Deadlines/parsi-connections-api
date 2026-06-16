using Connecions.Api.Dtos;
using Connecions.Api.Services;
using Connecions.Api.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Connecions.Api.Endpoints.PuzzleHandlers;

public class GetArchive
{
    public static async Task<IResult> Handler(
        HttpContext httpContext,
        PuzzleService puzzleService,
        GameStateService gameStateService,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10)
    {
        if (page < 1) page = 1;
        if (limit < 1 || limit > 100) limit = 10;

        var sessionIds = httpContext.GetGameSessionIds();
        var playedPuzzles = await puzzleService.GetPlayedPuzzles();

        if (!playedPuzzles.Any())
            return Results.NotFound("No archived puzzles available.");

        var playerStates = await gameStateService.GetAllPlayerGameStatesAsync(sessionIds);

        var archivedPuzzleDtos = playedPuzzles.Select(puzzle =>
        {
            var state = playerStates.FirstOrDefault(s => s.PuzzleId == puzzle.Id);
            return new ArchiveItemDto(
                PuzzleId: puzzle.Id,
                RemainingHealth: state == null ? 4 : state.RemainingHealth,
                SolvedCategories: state?.SolvedCategoryIds
                    .Split(',', StringSplitOptions.RemoveEmptyEntries).Length ?? 0,
                Outcome: state?.Outcome,
                LastUsedInDaily: puzzle.LastUsed!.Value);
        })
        .ToList();

        var total = archivedPuzzleDtos.Count();
        var totalPages = (int)MathF.Ceiling(total / (float)limit);

        var dtos = archivedPuzzleDtos
        .Skip((page - 1) * limit)
        .Take(limit)
        .ToList();

        return Results.Ok(new PaginatedResponse<ArchiveItemDto>(dtos, page, limit, total, totalPages));
    }
}
