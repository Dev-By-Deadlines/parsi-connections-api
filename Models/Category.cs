namespace Connecions.Api.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<string> Words { get; set; } = new();
}
