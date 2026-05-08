using Connecions.Api.Data;
using Connecions.Api.Dtos;
using Connecions.Api.Mapping;
using Connecions.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Connecions.Api.Endpoints;

public static class PuzzleEndpoints
{
    public static void MapPuzzleEndpoints(this WebApplication app)
    {
        const string GetPuzzleEndpointName = "GetPuzzle";

        var group = app.MapGroup("/puzzles");

        group.MapGet("/", GetPuzzles);
        group.MapGet("/{id}", GetPuzzle).WithName(GetPuzzleEndpointName);
        group.MapPost("/", CreatePuzzle);
        //
        // group.MapGet("/daily", GetDailyPuzzle);

        async Task<IResult> GetPuzzles(ConnectionsContext dbContext)
        {
            var puzzles = await dbContext.Puzzles
                .Include(p => p.Categories)
                .ThenInclude(c => c.Words)
                .ToListAsync();

            var dtos = puzzles.Select(p => p.ToAdminPuzzleDto()).ToList();

            return Results.Ok(dtos);
        }

        async Task<IResult> GetPuzzle(int id, ConnectionsContext dbContext)
        {
            var puzzle = await dbContext.Puzzles
                .Include(p => p.Categories)
                .ThenInclude(c => c.Words)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (puzzle is null) return Results.NotFound();

            return Results.Ok(puzzle.ToAdminPuzzleDto());
        }

        async Task<IResult> CreatePuzzle(CreatePuzzleDto createPuzzleDto, ConnectionsContext dbContext)
        {
            var puzzle = new Puzzle()
            {
                Categories = createPuzzleDto.Categories.Select(c => new Category()
                {
                    Name = c.Name,
                    Words = c.Words.Select(w => new Word()
                    {
                        Text = w.Text
                    }).ToList()
                }).ToList()
            };

            dbContext.Puzzles.Add(puzzle);
            await dbContext.SaveChangesAsync();

            var puzzleDto = puzzle.ToAdminPuzzleDto();

            return Results.CreatedAtRoute(GetPuzzleEndpointName, new { id = puzzle.Id }, puzzleDto);
        }
    }
}
