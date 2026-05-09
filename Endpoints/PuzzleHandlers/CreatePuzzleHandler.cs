using Connecions.Api.Data;
using Connecions.Api.Dtos;
using Connecions.Api.Mapping;
using Connecions.Api.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Connecions.Api.Endpoints.PuzzleHandlers;

public static class CreatePuzzleHandler
{
    public static async Task<IResult> Handler(CreatePuzzleDto createPuzzleDto, IValidator<CreatePuzzleDto> validator, ConnectionsContext dbContext)
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

        var puzzle = new Puzzle()
        {
            Categories = createPuzzleDto.Categories.Select(c => new Category()
            {
                Name = c.Name,
                Words = c.Words.Select(w => new Word()
                {
                    Text = w.Text
                }).ToList(),
            }).ToList()
        };

        foreach (var category in puzzle.Categories)
        {
            category.Puzzle = puzzle;
            foreach (var word in category.Words)
            {
                word.Category = category;
            }
        }

        dbContext.Puzzles.Add(puzzle);
        await dbContext.SaveChangesAsync();

        var puzzleDto = puzzle.ToAdminPuzzleDto();

        return Results.CreatedAtRoute(PuzzleEndpoints.GetPuzzleEndpointName, new { id = puzzle.Id }, puzzleDto);
    }
}
