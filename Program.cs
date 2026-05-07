using Connecions.Api.Data;
using Connecions.Api.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ConnectionsContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("Connections")));
var app = builder.Build();

app.MapPuzzleEndpoints();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ConnectionsContext>();
    db.Database.Migrate();
}
app.Run();
