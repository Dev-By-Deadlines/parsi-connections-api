using Connecions.Api.Models;
using Connecions.Api.Utils;

namespace Connecions.Api.Services;

public record GuessResult(Category? CorrectCategory, bool IsOneAway);

public class GuessService
{
    public GuessResult EvaluateGuess(Puzzle puzzle, List<string> words)
    {
        Category? correctCategory = null;
        bool isOneAway = false;

        foreach (var category in puzzle.Categories)
        {
            var wordSet = new HashSet<string>(category.Words.Select(w => w.Text));
            int commonCount = words.Count(w => wordSet.Contains(w));

            if (commonCount == 4)
            {
                correctCategory = category;
                break;
            }
            else if (commonCount == 3 && !isOneAway)
            {
                isOneAway = true;
            }
        }

        return new GuessResult(correctCategory, isOneAway);
    }

    public string[] GetGuessEmojiRow(Puzzle puzzle, List<string> words)
    {
        var row = new string[words.Count];

        for (int i = 0; i < words.Count; i++)
        {
            var matchingCategory = puzzle.Categories
                .FirstOrDefault(c => c.Words.Any(w => w.Text == words[i]));

            var emoji = matchingCategory is not null
                ? GameConstants.CategoryIndexToEmojiMap[puzzle.Categories.IndexOf(matchingCategory)]
                : "?";

            row[i] = emoji;
        }

        return row;
    }
}
