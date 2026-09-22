using LogisticsEngine.Application;
using LogisticsEngine.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
string dataDirectory = Path.Combine(builder.Environment.ContentRootPath, builder.Configuration["DataDirectory"] ?? "data");
builder.Services.AddApplication().AddInfrastructure(dataDirectory);
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

var app = builder.Build();
app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();
app.MapHealthChecks("/health");
await app.RunAsync();

public partial class Program;
