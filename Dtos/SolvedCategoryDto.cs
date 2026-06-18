namespace Connecions.Api.Dtos;

public record SolvedCategoryDto(string Name, int? categoryIndex, List<WordDto> Words) { }
