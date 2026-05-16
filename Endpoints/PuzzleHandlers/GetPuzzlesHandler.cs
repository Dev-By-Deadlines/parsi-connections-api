using Connecions.Api.Data;
using Connecions.Api.Dtos;
using Connecions.Api.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Connecions.Api.Endpoints.PuzzleHandlers;

public static class GetPuzzles
{
    public async static Task<IResult> Handler(
            ConnectionsContext dbContext,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10)
    {
        if (page < 1) page = 1;
        if (limit < 1 || limit > 100) limit = 10;

        var count = await dbContext.Puzzles.CountAsync();

        var puzzles = await dbContext.Puzzles
            .OrderBy(p => p.Id)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Include(p => p.Categories)
            .ThenInclude(c => c.Words)
            .ToListAsync();

        var dtos = puzzles.Select(p => p.ToAdminPuzzleDto()).ToList();
        var totalPages = (int)Math.Ceiling(count / (double)limit);

        return Results.Ok(new PaginatedResponse<AdminPuzzleDto>(dtos, page, limit, count, totalPages));
    }
}
