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

    public static List<Word> GetShuffledWords(this Puzzle puzzle)
    {
        return puzzle.Categories
            .SelectMany(c => c.Words)
            .Select(w => new Word { Text = w.Text })
            .OrderBy(_ => Guid.NewGuid())
            .ToList();
    }

    public static GameStateDto ToDto(this GameState state, Puzzle puzzle)
    {
        var solvedIds = state.SolvedCategoryIds
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .ToHashSet();

        var solvedCategories = puzzle.Categories
            .Where(c => solvedIds.Contains(c.Id))
            .Select(c => new SolvedCategoryDto(
                c.Name,
                c.Words.Select(w => new WordDto(w.Text)).ToList()
            ))
            .ToList();

        var unsolvedWords = puzzle.Categories
            .Where(c => !solvedIds.Contains(c.Id))
            .SelectMany(c => c.Words)
            .OrderBy(_ => Guid.NewGuid())    // re‑shuffle on every request
            .Select(w => w.Text)
            .ToList();

        return new GameStateDto
        {
            PuzzleId = state.PuzzleId,
            Outcome = state.Outcome,
            RemainingHealth = state.RemainingHealth,
            SolvedCategoryDtos = solvedCategories,
            UnSolvedWords = unsolvedWords
        };
    }
}
