namespace Connecions.Api.Dtos;

public record GuessResponseDto(bool Correct, string? CategoryName, List<string>? SolvedWords) { }
