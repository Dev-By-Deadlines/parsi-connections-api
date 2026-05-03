using Connecions.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPuzzleEndpoints();
app.Run();
