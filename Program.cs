using Serilog;

using Connecions.Api.Data;
using Connecions.Api.Endpoints;
using Connecions.Api.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Connecions.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .WriteTo.Console()
    .WriteTo.File(
            "/var/log/connections.log",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 7)
    .CreateLogger();

builder.Host.UseSerilog();

// Database
builder.Services.AddDbContext<ConnectionsContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Connections"))
    .ConfigureWarnings(warnings =>
        warnings.Ignore(RelationalEventId.MultipleCollectionIncludeWarning)));

// Validation
builder.Services.AddValidatorsFromAssemblyContaining<CreatePuzzleDtoValidator>();

// String Enums instead of int
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Custom Services
builder.Services.AddScoped<PuzzleService>();
builder.Services.AddScoped<GameStateService>();
builder.Services.AddScoped<GuessService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "Persian Connections API",
        Version = "v1",
        Description = "Endpoints for the Persian Connections game."
    });

    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "Enter your admin API key",
        Name = "X-Api-Key",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
    });
    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("ApiKey", document)] = new List<string>()
    });
});

// Admin API key
var adminKey = builder.Configuration["ApiKeys:AdminKey"]
    ?? Environment.GetEnvironmentVariable("AdminApiKey")
    ?? throw new InvalidOperationException("Admin API key not configured.");

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(
            "http://localhost:3000",
            "http://localhost:3001",
            "http://trollguys.ir",
            "https://trollguys.ir",
            "http://5.57.35.83:5001",
            "http://connections.trollguys.ir",
            "null"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        .WithExposedHeaders("*"));
});

// Rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
        RateLimitPartition.GetFixedWindowLimiter("GlobalPolicy", _ =>
            new FixedWindowRateLimiterOptions
            {
                PermitLimit = 60,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));
});

var app = builder.Build();

// Middleware pipeline
app.UseRateLimiter();
app.UseCors();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Persian Connections API v1");
    c.RoutePrefix = "docs";
});

// Map endpoints (player + admin)
app.MapPuzzleEndpoints(adminKey);

// Auto-migrate database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ConnectionsContext>();
    db.Database.Migrate();
}

app.Run();
