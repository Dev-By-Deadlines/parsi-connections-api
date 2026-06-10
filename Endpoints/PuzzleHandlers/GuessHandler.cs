using Connecions.Api.Dtos;
using Connecions.Api.Mapping;
using Connecions.Api.Models;
using Connecions.Api.Services;
using Connecions.Api.Utils;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Connecions.Api.Data;

namespace Connecions.Api.Endpoints.PuzzleHandlers;

public class Guess
{
    public static async Task<IResult> Handler(
        int id,
        IValidator<GuessDto> validator,
        GuessDto guess,
        ConnectionsContext dbContext,
        HttpContext httpContext,
        PuzzleService puzzleService,
        GuessService guessService,
        GameStateService gameStateService,
        ILogger<Guess> logger)
    {
        var cookieName = GameConstants.SessionCookieName;
        if (!httpContext.Request.Cookies.TryGetValue(cookieName, out var cookieValue))
            return Results.BadRequest("No active game sessions.");

        var sessionIds = cookieValue
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .ToList();

        var state = await dbContext.GameStates
            .FirstOrDefaultAsync(gs => sessionIds.Contains(gs.SessionId) && gs.PuzzleId == id);

        if (state is null)
            return Results.BadRequest("No session found for this puzzle.");

        if (state.Outcome != Outcomes.Playing)
            return Results.BadRequest("Game is already over.");

        var validationResult = await validator.ValidateAsync(guess);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(
                validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()));

        var puzzle = await puzzleService.GetPuzzleWithCategoriesAsync(id);
        if (puzzle is null)
            return Results.NotFound("Puzzle not found.");

        var result = guessService.EvaluateGuess(puzzle, guess.Words);

        if (result.CorrectCategory is not null)
        {
            gameStateService.ApplyCorrectGuess(state, result.CorrectCategory);
            await gameStateService.SaveAsync();
            logger.LogInformation("Correct guess for puzzle {PuzzleId} -- Category: {CategoryName}", puzzle.Id, result.CorrectCategory.Name);
        }
        else
        {
            gameStateService.ApplyWrongGuess(state);
            await gameStateService.SaveAsync();
            if (state.Outcome == Outcomes.Lost)
                logger.LogInformation("Player lost puzzle {PuzzleId}", puzzle.Id);
        }

        return Results.Ok(new GuessResponseDto(
            Correct: result.CorrectCategory is not null,
            OneAway: result.IsOneAway,
            GameStateDto: state.ToDto(puzzle)));
    }
}
