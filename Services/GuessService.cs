using Connecions.Api.Models;

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
}
