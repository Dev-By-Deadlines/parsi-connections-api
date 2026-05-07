using Connecions.Api.Dtos;
using Connecions.Api.Models;

namespace Connecions.Api.Mapping;

public static class PuzzleMapping
{

    public static AdminPuzzleDto ToAdminPuzzleDto(this Puzzle puzzle)
    {
        return new AdminPuzzleDto(
                puzzle.Id,
                puzzle.LastUsed,
                puzzle.Categories.Select(c => new AdminCategoryDto(
                        c.Id,
                        c.Name,
                        c.Words.Select(w => new AdminWordDto(w.Id, w.Text)).ToList()
                        )).ToList()
                );
    }
}

