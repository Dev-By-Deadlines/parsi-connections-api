using Connecions.Api.Data;

namespace Connecions.Api.Endpoints.HealthCheck;

public static class HealthCheckHandler
{
    public async static Task<IResult> Handler(ConnectionsContext dbContext)
    {
        var canConnect = await dbContext.Database.CanConnectAsync();

        if (!canConnect)
            return Results.Problem("Can't reach the database", statusCode: 503);
        return Results.Ok(new { status = "Healthy", db = "Ok" });
    }
}
