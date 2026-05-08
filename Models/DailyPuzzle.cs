namespace Connecions.Api.Models;

public class DailyPuzzle
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public Puzzle Puzzle { get; set; } = null!;
    public int PuzzleId { get; set; }
}
