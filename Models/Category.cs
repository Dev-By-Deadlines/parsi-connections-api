namespace Connecions.Api.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Word> Words { get; set; } = new();

    public int PuzzleId { get; set; }
    public Puzzle Puzzle { get; set; } = null!;
}
