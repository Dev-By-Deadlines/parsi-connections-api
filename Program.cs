using Connecions.Api.Data;
using Connecions.Api.Endpoints;
using Connecions.Api.Validators;
using Scalar.AspNetCore;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ConnectionsContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("Connections")));

builder.Services.AddValidatorsFromAssemblyContaining<CreatePuzzleDtoValidator>();

builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader());
});

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
        RateLimitPartition.GetFixedWindowLimiter("GlobalPolicy", _ =>
            new FixedWindowRateLimiterOptions
            {
                PermitLimit = 30,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));
});

var app = builder.Build();

app.UseRateLimiter();
app.UseCors();
app.MapPuzzleEndpoints();
app.MapOpenApi();
app.MapScalarApiReference();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ConnectionsContext>();
    db.Database.Migrate();
}

app.Run();
