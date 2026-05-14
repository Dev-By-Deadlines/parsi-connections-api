using Connecions.Api.Data;
using Connecions.Api.Dtos;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Connecions.Api.Endpoints.PuzzleHandlers;

public static class Guess
{
    public static async Task<IResult> Handler(
        int id,
        IValidator<GuessDto> validator,
        GuessDto guess,
        ConnectionsContext dbContext)
    {
        var validationResult = await validator.ValidateAsync(guess);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(
                validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()));
        }

        var puzzle = await dbContext.Puzzles
            .Include(p => p.Categories)
                .ThenInclude(c => c.Words)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (puzzle is null)
            return Results.NotFound("Puzzle not found.");

        foreach (var category in puzzle.Categories)
        {
            var categoryWordSet = new HashSet<string>(category.Words.Select(w => w.Text));
            int commonCount = guess.Words.Count(w => categoryWordSet.Contains(w));

            if (commonCount == 4)
            {
                return Results.Ok(new GuessResponseDto(
                    Correct: true,
                    CategoryName: category.Name,
                    SolvedWords: category.Words.Select(w => w.Text).ToList(),
                    OneAway: false));
            }
            else if (commonCount == 3)
            {
                return Results.Ok(new GuessResponseDto(
                    Correct: false,
                    CategoryName: null,
                    SolvedWords: null,
                    OneAway: true));
            }
        }

        return Results.Ok(new GuessResponseDto(
            Correct: false,
            CategoryName: null,
            SolvedWords: null,
            OneAway: false));
    }
}
