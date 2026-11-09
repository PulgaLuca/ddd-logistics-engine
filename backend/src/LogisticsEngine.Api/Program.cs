using LogisticsEngine.Application;
using LogisticsEngine.Api.Authentication;
using LogisticsEngine.Infrastructure;
using LogisticsEngine.Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using System.Reflection;
using System.Text.Json.Serialization;

/// <summary>Avvia la Web API e compone i layer tramite dependency injection.</summary>
public partial class Program
{
    private const string _developmentCorsPolicy = "DevelopmentBrowserClients";

    public static void Main(string[] args)
    {
        // Legge .env prima che il configuration builder raccolga le variabili d'ambiente.
        DotEnvFile.LoadFromRepositoryAncestors();
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

        builder.Services.AddAuthentication(BearerTokenAuthenticationDefaults.Scheme)
            .AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions,
                BearerTokenAuthenticationHandler>(BearerTokenAuthenticationDefaults.Scheme, _ => { });

        builder.Services.AddAuthorization();

        builder.Services.AddHealthChecks();

        builder.Services.AddCors(options => options.AddPolicy(_developmentCorsPolicy, policy =>
            policy.WithOrigins("http://localhost:5003", "https://localhost:7259", "http://localhost:5173")
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
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Inserisci il token restituito da POST /api/v1/users/authenticate."
            });
            options.OperationFilter<BearerSecurityRequirementsOperationFilter>();

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

        // Il percorso JSON serve al provider locale; il provider Mongo usa il database indicato da MONGODB_DATABASE.
        string dataDirectory = builder.Configuration["Storage:DataDirectory"]
            ?? ResolveDataDirectory(builder.Environment.ContentRootPath);

        string signingKey = builder.Configuration["Authentication:SigningKey"]
            ?? throw new InvalidOperationException("Configure Authentication:SigningKey with at least 32 bytes.");

        string? storageProvider = builder.Configuration["STORAGE_PROVIDER"];
        // Una URI presente abilita Atlas per comodità; STORAGE_PROVIDER=Json permette di forzare il provider su file.

        bool useMongoDb = string.Equals(storageProvider, "MongoDb", StringComparison.OrdinalIgnoreCase)
            || (string.IsNullOrWhiteSpace(storageProvider)
                && !string.IsNullOrWhiteSpace(builder.Configuration["MONGODB_URI"]));

        MongoDbOptions? mongoDbOptions = useMongoDb ? MongoDbOptions.FromEnvironment() : null;

        builder.Services.AddInfrastructure(dataDirectory, signingKey, mongoDbOptions, builder.Configuration["Authentication:Google:ClientId"]);

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
            app.UseCors(_developmentCorsPolicy);
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
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
