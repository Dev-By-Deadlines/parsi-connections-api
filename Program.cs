using Connecions.Api.Data;
using Connecions.Api.Endpoints;
using Connecions.Api.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ConnectionsContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("Connections")));

builder.Services.AddValidatorsFromAssemblyContaining<CreatePuzzleDtoValidator>();

var app = builder.Build();

app.MapPuzzleEndpoints();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ConnectionsContext>();
    db.Database.Migrate();
}
app.Run();
