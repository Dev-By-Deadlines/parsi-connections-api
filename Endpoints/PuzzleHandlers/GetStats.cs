using Connecions.Api.Data;
using Connecions.Api.Dtos;
using Connecions.Api.Models;
using Connecions.Api.Utils;
using Microsoft.EntityFrameworkCore;

namespace Connecions.Api.Endpoints.PuzzleHandlers;

public class GetStats
{
    public static async Task<IResult> Handler(
        int id,
        HttpContext httpContext,
        ConnectionsContext dbContext)
    {
        var sessionIds = httpContext.GetGameSessionIds();

        var state = await dbContext.GameStates
            .FirstOrDefaultAsync(gs => sessionIds.Contains(gs.SessionId) && gs.PuzzleId == id);

        if (state is null)
            return Results.NotFound("No session found for this puzzle.");

        if (state.Outcome == Outcomes.Playing)
            return Results.BadRequest("Game is not finished yet.");

        var allFinished = await dbContext.GameStates
            .Where(gs => gs.PuzzleId == id && gs.Outcome != Outcomes.Playing)
            .ToListAsync();

        var totalPlayers = allFinished.Count;
        var winners = allFinished.Count(s => s.Outcome == Outcomes.Won);
        var winRate = totalPlayers > 0 ? Math.Round((double)winners / totalPlayers * 100, 1) : 0;
        var averageHealth = totalPlayers > 0 ? Math.Round(allFinished.Average(s => s.RemainingHealth), 1) : 0;
        var percentile = totalPlayers > 1
            ? Math.Round((double)allFinished.Count(s => s.RemainingHealth < state.RemainingHealth) / (totalPlayers - 1) * 100, 1)
            : 100;

        var allEmojis = state.GuessGrid;
        var grid = new List<string>();

        for (int i = 0; i < allEmojis.Count; i += 4)
        {
            grid.Add(string.Join("", allEmojis.Skip(i).Take(4)));
        }

        return Results.Ok(new StatsDto(
            TotalPlayers: totalPlayers,
            WinRate: winRate,
            AverageRemainingHealth: averageHealth,
            PlayerPercentile: percentile,
            PlayerHealth: state.RemainingHealth,
            PlayerOutcome: state.Outcome,
            GuessGrid: grid
        ));
    }
}
