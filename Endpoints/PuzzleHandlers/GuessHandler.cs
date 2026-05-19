using Connecions.Api.Data;
using Connecions.Api.Dtos;
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

        var state = await dbContext.GameState.FirstOrDefaultAsync(gs => gs.SessionId == sessionId);
        if (state is null || state.PuzzleId != id)
            return Results.BadRequest("Invalid session.");

        // Already over?
        if (state.RemainingHealth <= 0)
            return Results.BadRequest("Game already lost. Start a new day.");

        var solvedIds = state.SolvedCategoryIds
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .ToHashSet();

        if (solvedIds.Count == 4)
            return Results.BadRequest("Game already won. Come back tomorrow.");

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
            if (solvedIds.Contains(category.Id))
                continue;

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
            solvedIds.Add(correctCategory.Id);
            state.SolvedCategoryIds = string.Join(',', solvedIds);
            bool gameWon = solvedIds.Count == 4;

            if (gameWon)
            {
                await dbContext.SaveChangesAsync();
                return Results.Ok(new GuessResponseDto(
                    Correct: true,
                    CategoryName: correctCategory.Name,
                    OneAway: false,
                    SolvedWords: correctCategory.Words.Select(w => w.Text).ToList(),
                    RemainingHealth: state.RemainingHealth,
                    Outcome: "win"));
            }

            await dbContext.SaveChangesAsync();
            return Results.Ok(new GuessResponseDto(
                Correct: true,
                CategoryName: correctCategory.Name,
                OneAway: false,
                SolvedWords: correctCategory.Words.Select(w => w.Text).ToList(),
                RemainingHealth: state.RemainingHealth));
        }

        // Wrong guess
        state.RemainingHealth--;
        await dbContext.SaveChangesAsync();

        if (state.RemainingHealth <= 0)
        {
            var allCategories = puzzle.Categories.Select(c => new
            {
                c.Name,
                Words = c.Words.Select(w => w.Text).ToList()
            }).ToList();

            return Results.Ok(new GuessResponseDto(
                Correct: false,
                CategoryName: null,
                OneAway: false,
                SolvedWords: null,
                RemainingHealth: 0,
                Outcome: "loss",
                AllCategories: allCategories));
        }

        if (oneAwayCategory is not null)
        {
            return Results.Ok(new GuessResponseDto(
                Correct: false,
                CategoryName: null,
                SolvedWords: null,
                OneAway: true,
                RemainingHealth: state.RemainingHealth));
        }

        return Results.Ok(new GuessResponseDto(
            Correct: false,
            CategoryName: null,
            OneAway: false,
            SolvedWords: null,
            RemainingHealth: state.RemainingHealth));
    }
}
