using Connecions.Api.Data;
using Connecions.Api.Dtos;
using Connecions.Api.Mapping;
using Connecions.Api.Models;
using Connecions.Api.Utils;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Connecions.Api.Endpoints.PuzzleHandlers;

public static class Guess
{
    public static async Task<IResult> Handler(
        int id,
        IValidator<GuessDto> validator,
        GuessDto guess,
        ConnectionsContext dbContext,
        HttpContext httpContext)
    {
        var cookieName = GameConstants.SessionCookieName;
        if (!httpContext.Request.Cookies.TryGetValue(cookieName, out var sessionId))
            return Results.BadRequest("No active game session.");

        var state = await dbContext.GameStates.FirstOrDefaultAsync(gs => gs.SessionId == sessionId);
        if (state is null || state.PuzzleId != id)
            return Results.BadRequest("Invalid session.");

        if (state.Outcome != Outcomes.Playing)
            return Results.BadRequest("Game is already over.");

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

        Category? correctCategory = null;
        Category? oneAwayCategory = null;

        foreach (var category in puzzle.Categories)
        {
            var categoryWordSet = new HashSet<string>(category.Words.Select(w => w.Text));
            int commonCount = guess.Words.Count(w => categoryWordSet.Contains(w));

            if (commonCount == 4)
            {
                correctCategory = category;
                break;
            }
            else if (commonCount == 3 && oneAwayCategory is null)
            {
                oneAwayCategory = category;
            }
        }

        if (correctCategory is not null)
        {
            // Add the solved category ID
            var solvedIds = state.SolvedCategoryIds
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToHashSet();
            solvedIds.Add(correctCategory.Id);
            state.SolvedCategoryIds = string.Join(',', solvedIds);

            bool gameWon = solvedIds.Count == 4;
            state.Outcome = gameWon ? Outcomes.Won : Outcomes.Playing;

            await dbContext.SaveChangesAsync();

            var dto = state.ToDto(puzzle);
            return Results.Ok(new GuessResponseDto(
                Correct: true,
                OneAway: false,
                GameStateDto: dto));
        }

        // Wrong guess
        state.RemainingHealth--;

        if (state.RemainingHealth == 0)
        {
            state.Outcome = Outcomes.Lost;
        }

        await dbContext.SaveChangesAsync();

        var resultDto = state.ToDto(puzzle);
        return Results.Ok(new GuessResponseDto(
            Correct: false,
            OneAway: oneAwayCategory is not null,
            GameStateDto: resultDto));
    }
}
