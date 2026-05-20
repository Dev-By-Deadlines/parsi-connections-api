using Connecions.Api.Models;

namespace Connecions.Api.Dtos;

public record class GameStateDto()
{
    public int PuzzleId { get; set; }
    public Outcomes Outcome { get; set; }
    public int RemainingHealth { get; set; }
    public List<string> UnSolvedWords { get; set; } = new();
    public List<SolvedCategoryDto> SolvedCategoryDtos { get; set; } = new();
}
