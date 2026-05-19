namespace Connecions.Api.Dtos;

public record class PlayerPuzzleDto(int Id, List<string> Words)
{
    public int RemainingHealth { get; set; }
}
