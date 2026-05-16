using Connecions.Api.Data;
using Connecions.Api.Dtos;
using Connecions.Api.Mapping;
using Connecions.Api.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Connecions.Api.Endpoints.PuzzleHandlers;

public static class UpdatePuzzle
{
    public static async Task<IResult> Handler(int id, CreatePuzzleDto createPuzzleDto, IValidator<CreatePuzzleDto> validator, ConnectionsContext dbContext)
    {
        var validationResult = await validator.ValidateAsync(createPuzzleDto);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(
                    validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
                    );
        }

        var puzzle = await dbContext.Puzzles
            .Include(p => p.Categories)
            .ThenInclude(c => c.Words)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (puzzle is null) return Results.NotFound();

        dbContext.Categories.RemoveRange(puzzle.Categories);

        puzzle.Categories = createPuzzleDto.Categories.Select(c => new Category
        {
            Name = c.Name,
            Words = c.Words.Select(w => new Word
            {
                Text = w.Text
            }).ToList()
        }).ToList();


        // Set back‑references
        foreach (var category in puzzle.Categories)
        {
            category.Puzzle = puzzle;
            foreach (var word in category.Words)
            {
                word.Category = category;
            }
        }

        await dbContext.SaveChangesAsync();

        return Results.Ok(puzzle.ToAdminPuzzleDto());
    }
}
