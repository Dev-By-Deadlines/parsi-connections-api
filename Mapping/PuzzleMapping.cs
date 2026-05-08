using Connecions.Api.Dtos;
using Connecions.Api.Models;

namespace Connecions.Api.Mapping;

public static class PuzzleMapping
{
    public static AdminPuzzleDto ToAdminPuzzleDto(this Puzzle puzzle)
    {
        return new AdminPuzzleDto(
                puzzle.Id,
                puzzle.LastUsed,
                puzzle.Categories.Select(c => new AdminCategoryDto(
                        c.Id,
                        c.Name,
                        c.Words.Select(w => new AdminWordDto(w.Id, w.Text)).ToList()
                        )).ToList()
                );
    }

    public static PlayerPuzzleDto ToPlayerPuzzleDto(this Puzzle puzzle)
    {
        var wordStrings = new List<string>();

        puzzle.Categories
            .Shuffle()
            .SelectMany(c => c.Words)
            .Shuffle()
            .ToList()
            .ForEach(w => wordStrings.Add(w.Text));

        return new PlayerPuzzleDto(puzzle.Id, wordStrings);
    }
}

