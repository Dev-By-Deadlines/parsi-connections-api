namespace Connecions.Api.Dtos;

public record GuessResponseDto(
        bool Correct,
        bool OneAway,
        GameStateDto GameStateDto
        );
