using Connecions.Api.Models;

namespace Connecions.Api.Dtos;

public record ArchiveItemDto(
        int PuzzleId,
        int RemainingHealth,
        int SolvedCategories,
        Outcomes? Outcome,
        DateOnly LastUsedInDaily
        )
{ }
