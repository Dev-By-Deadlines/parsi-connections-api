namespace Connecions.Api.Dtos;

public record GuessResponseDto(
        bool Correct,
        string? CategoryName,
        List<string>? SolvedWords,
        bool OneAway,
        string? Outcome = null,
        int? RemainingHealth = null,
        object? AllCategories = null
        );
