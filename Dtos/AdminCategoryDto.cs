namespace Connecions.Api.Dtos;

public record AdminCategoryDto(int Id, string Name, List<AdminWordDto> Words) { }
