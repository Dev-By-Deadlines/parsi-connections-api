namespace Connecions.Api.Models;

public class Puzzle
{
    public int Id { get; set; }
    public DateOnly? LastUsed { get; set; }
    public List<Category> Categories { get; set; } = new();
}
