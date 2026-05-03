namespace Connecions.Api.Endpoints;

public static class PuzzleEndpoints
{
    public static void MapPuzzleEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/puzzle");

        group.MapGet("/", GetPuzzles);

        async Task<IResult> GetPuzzles()
        {

        }
    }
}
