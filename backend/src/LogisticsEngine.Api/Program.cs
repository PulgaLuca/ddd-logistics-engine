using LogisticsEngine.Application;
using LogisticsEngine.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using System.Reflection;
using System.Text.Json.Serialization;

/// <summary>Avvia la Web API e compone i layer tramite dependency injection.</summary>
public class Program
{
    private const string DevelopmentCorsPolicy = "DevelopmentBrowserClients";

    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Verifica il grafo DI all'avvio: servizi non registrati e dipendenze incompatibili emergono subito.
        builder.Host.UseDefaultServiceProvider(options =>
        {
            options.ValidateOnBuild = true;
            options.ValidateScopes = true;
        });

        // I controller HTTP sono adapter sottili; i casi d'uso vengono inviati con ISender a MediatR.
        builder.Services.AddControllers().AddJsonOptions(options =>
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        builder.Services.AddAuthorization();
        builder.Services.AddHealthChecks();
        builder.Services.AddCors(options => options.AddPolicy(DevelopmentCorsPolicy, policy =>
            policy.WithOrigins("http://localhost:5003", "https://localhost:7259")
                .AllowAnyHeader()
                .AllowAnyMethod()));
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "LogisticsEngine API",
                Version = "v1",
                Description = "API version v1 for hubs, shipments and domain algorithms."
            });

            // Include endpoint summaries and response documentation in the generated OpenAPI document.
            string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }
        });

        // Application registra request handler MediatR, validator e domain service.
        builder.Services.AddApplication();

        // Infrastructure collega i repository concreti JSON alle interfacce del Domain.
        string dataDirectory = ResolveDataDirectory(builder.Environment.ContentRootPath);
        builder.Services.AddInfrastructure(dataDirectory);

        WebApplication app = builder.Build();

        // Documentazione interattiva e OpenAPI restano esposte solo in sviluppo.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "LogisticsEngine API v1");
                options.RoutePrefix = "swagger";
            });
        }

        // Routing selects endpoint metadata before CORS; the dev policy also covers the HTTP-to-HTTPS redirect.
        app.UseRouting();
        if (app.Environment.IsDevelopment())
        {
            app.UseCors(DevelopmentCorsPolicy);
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.MapHealthChecks("/health");
        app.Run();
    }

    private static string ResolveDataDirectory(string contentRootPath)
    {
        DirectoryInfo? directory = new(contentRootPath);
        while (directory is not null)
        {
            bool isBackendRoot = Directory.Exists(Path.Combine(directory.FullName, "src"))
                && Directory.Exists(Path.Combine(directory.FullName, "tests"));
            if (isBackendRoot)
            {
                return Path.Combine(directory.FullName, "data");
            }

            directory = directory.Parent;
        }

        // In un ambiente pubblicato senza sorgenti, usa la cartella data accanto all'applicazione.
        return Path.Combine(contentRootPath, "data");
    }
}
