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
        // Parse normally solved IDs
        var solvedIds = state.SolvedCategoryIds
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .ToHashSet();

        List<SolvedCategoryDto> solvedCategories;
        List<string> unsolvedWords;

        if (state.Outcome == Outcomes.Lost || state.Outcome == Outcomes.Won)
        {
            // Game over – reveal everything
            solvedCategories = puzzle.Categories
                .Select(c => new SolvedCategoryDto(
                    c.Name,
                    c.Words.Select(w => new WordDto(w.Text)).ToList()
                ))
                .ToList();
            unsolvedWords = new List<string>();   // nothing left to guess
        }
        else
        {
            // Normal play – only show solved categories and remaining words
            solvedCategories = puzzle.Categories
                .Where(c => solvedIds.Contains(c.Id))
                .Select(c => new SolvedCategoryDto(
                    c.Name,
                    c.Words.Select(w => new WordDto(w.Text)).ToList()
                ))
                .ToList();
            unsolvedWords = puzzle.Categories
                .Where(c => !solvedIds.Contains(c.Id))
                .SelectMany(c => c.Words)
                .OrderBy(_ => Guid.NewGuid())
                .Select(w => w.Text)
                .ToList();
        }

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
