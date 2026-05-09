using Connecions.Api.Data;
using Connecions.Api.Endpoints;
using Connecions.Api.Validators;
using Scalar.AspNetCore;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ConnectionsContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("Connections")));

builder.Services.AddValidatorsFromAssemblyContaining<CreatePuzzleDtoValidator>();

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapPuzzleEndpoints();

app.MapOpenApi();
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ConnectionsContext>();
    db.Database.Migrate();
}

app.Run();
