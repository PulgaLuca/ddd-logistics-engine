using LogisticsEngine.Domain.Hubs;
using LogisticsEngine.Domain.Shipments;
using LogisticsEngine.Domain.Users;
using LogisticsEngine.Application.Users;
using LogisticsEngine.Infrastructure.Users;
using LogisticsEngine.Infrastructure.Hubs;
using LogisticsEngine.Infrastructure.Shipments;
using LogisticsEngine.Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace LogisticsEngine.Infrastructure;

// Extension method for DI container for earch layer
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string dataDirectory,
        string? tokenSigningKey = null,
        MongoDbOptions? mongoDbOptions = null)
    {
        if (mongoDbOptions is null)
        {
            // JSON resta il provider locale predefinito quando non è stata configurata una connessione Atlas.
            services.AddSingleton<IHubRepository>(new JsonHubRepository(dataDirectory));
            services.AddSingleton<IHubConnectionRepository>(new JsonHubConnectionRepository(dataDirectory));
            services.AddSingleton<IShipmentRepository>(new JsonShipmentRepository(dataDirectory));
            services.AddSingleton<IUserRepository>(new JsonUserRepository(dataDirectory));
        }
        else
        {
            // MongoClient possiede un pool di connessioni: registriamo una sola istanza riutilizzata da tutti i repository.
            MongoClientSettings settings = MongoClientSettings.FromConnectionString(mongoDbOptions.ConnectionString);
            // Stable API v1 riduce l'impatto di cambiamenti server sulle chiamate del driver.
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            var client = new MongoClient(settings);
            IMongoDatabase database = client.GetDatabase(mongoDbOptions.DatabaseName);

            services.AddSingleton<IMongoClient>(client);
            services.AddSingleton(database);
            services.AddSingleton<IHubRepository, MongoHubRepository>();
            services.AddSingleton<IHubConnectionRepository, MongoHubConnectionRepository>();
            services.AddSingleton<IShipmentRepository, MongoShipmentRepository>();
            services.AddSingleton<IUserRepository, MongoUserRepository>();
        }

        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddSingleton<IUserAuthenticationService, JsonUserAuthenticationService>();
        services.AddSingleton<IUserAuthorizationService, DomainUserAuthorizationService>();
        string signingKey = tokenSigningKey ?? Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));
        var accessTokenService = new HmacUserAccessTokenService(signingKey);
        services.AddSingleton<IUserAccessTokenService>(accessTokenService);
        services.AddSingleton<IUserAccessTokenValidator>(accessTokenService);
        return services;
    }
}
