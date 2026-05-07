using Connecions.Api.Models;

namespace Connecions.Api.Dtos;

public record CreateCategoryDto(
        string Name,
        List<CreateWordDto> Words
        )
{ }
