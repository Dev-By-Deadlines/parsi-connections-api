using Connecions.Api.Data;
using Connecions.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Connecions.Api.Endpoints.PuzzleHandlers;

public static class GuessHandler
{
    public static async Task<IResult> Handler(int id, GuessDto guess, ConnectionsContext dbContext)
    {
        if (guess.Words.Count != 4)
            return Results.BadRequest("Exactly 4 words are required.");

        if (guess.Words.Distinct().Count() != 4)
            return Results.BadRequest("Guess must contain four different words.");

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
