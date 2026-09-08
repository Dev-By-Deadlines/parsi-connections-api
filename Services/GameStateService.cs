using Connecions.Api.Data;
using Connecions.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Connecions.Api.Services;

public class GameStateService(ConnectionsContext dbContext)
{
    public async Task<GameState> GetOrCreateForPuzzleAsync(List<string> sessionIds, Puzzle puzzle)
    {
        GameState? state = null;

        foreach (var sessionId in sessionIds)
        {
            state = await dbContext.GameStates
                .FirstOrDefaultAsync(gs =>
                    gs.SessionId == sessionId &&
                    gs.PuzzleId == puzzle.Id);

            if (state is not null)
                break;
        }

        if (state is null)
        {
            state = new GameState
            {
                SessionId = GenerateSessionId(),
                PuzzleId = puzzle.Id,
                Outcome = Outcomes.Playing,
                RemainingHealth = 4,
                SolvedCategoryIds = "",
                WordOrder = GenerateWordOrder(puzzle)
            };

            dbContext.GameStates.Add(state);
        }

        RecalculateOutcome(state);
        return state;
    }

    public async Task<List<GameState>> GetAllPlayerGameStatesAsync(List<string> sessionIds)
    {
        var states = await dbContext.GameStates
            .ToListAsync();

        return states
            .Where(gs => sessionIds.Contains(gs.SessionId))
            .ToList();
    }

    private static readonly char[] Chars = "abcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
    public static string GenerateSessionId()
    {
        return new string(Enumerable.Range(0, 8)
            .Select(_ => Chars[Random.Shared.Next(Chars.Length)])
            .ToArray());
    }

    private static string GenerateWordOrder(Puzzle puzzle) =>
        string.Join(',', puzzle.Categories
            .SelectMany(c => c.Words)
            .OrderBy(_ => Guid.NewGuid())
            .Select(w => w.Id));

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
