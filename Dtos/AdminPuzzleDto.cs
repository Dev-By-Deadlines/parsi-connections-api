namespace Connecions.Api.Dtos;

public record class AdminPuzzleDto(int Id, DateOnly? LastUsed, List<AdminCategoryDto> Categories) { }
