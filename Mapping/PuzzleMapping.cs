using Connecions.Api.Dtos;
using Connecions.Api.Models;

namespace Connecions.Api.Mapping;

public static class PuzzleMapping
{
    public static PlayerPuzzleDto ToPlayerPuzzleDto(this Puzzle puzzle)
    {
        return new PlayerPuzzleDto(puzzle.Id, puzzle.Categories.SelectMany(c => c.Words)
                .OrderBy(_ => Guid.NewGuid())
                .ToList());
    }

    public static AdminPuzzleDto ToAdminPuzzleDto(this Puzzle puzzle)
    {
        return new AdminPuzzleDto(puzzle.Id, puzzle.Categories);
    }
}
