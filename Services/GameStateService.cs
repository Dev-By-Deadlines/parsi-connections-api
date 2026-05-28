using Connecions.Api.Data;
using Connecions.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Connecions.Api.Services;

public class GameStateService(ConnectionsContext dbContext)
{
    public async Task<GameState> GetOrCreateAsync(string sessionId, Puzzle puzzle)
    {
        var state = await dbContext.GameStates
            .FirstOrDefaultAsync(gs => gs.SessionId == sessionId);

        if (state is null)
        {
            state = new GameState
            {
                SessionId = sessionId,
                PuzzleId = puzzle.Id,
                Outcome = Outcomes.Playing,
                RemainingHealth = 4,
                SolvedCategoryIds = ""
            };
            dbContext.GameStates.Add(state);
        }
        else if (state.PuzzleId != puzzle.Id) // new day
        {
            state.PuzzleId = puzzle.Id;
            state.RemainingHealth = 4;
            state.Outcome = Outcomes.Playing;
            state.SolvedCategoryIds = "";
        }

        RecalculateOutcome(state);
        return state;
    }

    public void ApplyCorrectGuess(GameState state, Category correctCategory)
    {
        var solvedIds = ParseSolvedIds(state);
        solvedIds.Add(correctCategory.Id);
        state.SolvedCategoryIds = string.Join(',', solvedIds);
        RecalculateOutcome(state);
    }

    public void ApplyWrongGuess(GameState state)
    {
        state.RemainingHealth--;
        RecalculateOutcome(state);
    }

    public async Task SaveAsync() => await dbContext.SaveChangesAsync();

    private static void RecalculateOutcome(GameState state)
    {
        if (state.RemainingHealth <= 0)
            state.Outcome = Outcomes.Lost;
        else if (ParseSolvedIds(state).Count >= 4)
            state.Outcome = Outcomes.Won;
    }

    private static HashSet<int> ParseSolvedIds(GameState state) =>
        state.SolvedCategoryIds
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .ToHashSet();
}
