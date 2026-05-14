using Connecions.Api.Data;
using Connecions.Api.Dtos;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Connecions.Api.Endpoints.PuzzleHandlers;

public static class Guess
{
    public static async Task<IResult> Handler(int id, IValidator<GuessDto> validator, GuessDto guess, ConnectionsContext dbContext)
    {
        var validationResult = await validator.ValidateAsync(guess);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(
                    validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
                    );
        }

        var puzzle = await dbContext.Puzzles
            .Include(p => p.Categories)
            .ThenInclude(c => c.Words)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (puzzle is null)
            return Results.NotFound("Puzzle not found.");

        var guessSet = new HashSet<string>(guess.Words);

        var match = puzzle.Categories.FirstOrDefault(c =>
            new HashSet<string>(c.Words.Select(w => w.Text))
                .SetEquals(guessSet));

        if (match is not null)
        {
            return Results.Ok(new GuessResponseDto(
                true,
                match.Name,
                match.Words.Select(w => w.Text).ToList()
            ));
        }

        return Results.Ok(new GuessResponseDto(false, null, null));
    }
}
