namespace Connecions.Api.Models;

public class GameState
{
    public int Id { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public int PuzzleId { get; set; }
    public int RemainingHealth { get; set; } = 4;
    public string SolvedCategoryIds { get; set; } = "";
}
