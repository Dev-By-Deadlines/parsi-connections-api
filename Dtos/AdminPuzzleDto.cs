using Connecions.Api.Models;

namespace Connecions.Api.Dtos;

public record class AdminPuzzleDto(int Id, List<Category> Categories) { }
