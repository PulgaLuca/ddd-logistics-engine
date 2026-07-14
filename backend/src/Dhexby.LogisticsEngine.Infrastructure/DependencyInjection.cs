using Dhexby.LogisticsEngine.Application.Abstractions.Authentication;
using Dhexby.LogisticsEngine.Domain.Hubs;
using Dhexby.LogisticsEngine.Domain.Orders;
using Dhexby.LogisticsEngine.Domain.Routing;
using Dhexby.LogisticsEngine.Domain.Shipments;
using Dhexby.LogisticsEngine.Domain.Users;
using Dhexby.LogisticsEngine.Domain.Zones;
using Dhexby.LogisticsEngine.Infrastructure.Authentication;
using Dhexby.LogisticsEngine.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dhexby.LogisticsEngine.Infrastructure;

/// <summary>
/// Composition root del layer Infrastructure: lega i contratti del dominio
/// alle implementazioni concrete. Il provider di persistenza (JSON o MongoDB)
/// è scelto in base alla configurazione: cambiando "Persistence:Provider" si
/// sostituisce l'intero storage senza toccare Application, Domain o Api.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Servizi di autenticazione, indipendenti dal provider di persistenza.
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        string? provider = configuration.GetSection(PersistenceOptions.SectionName)["Provider"];

        bool useMongo = string.Equals(provider, PersistenceOptions.MongoProvider, StringComparison.OrdinalIgnoreCase);

        return useMongo
            ? services.AddMongoPersistence(configuration)
            : services.AddJsonPersistence(configuration);
    }

    private static IServiceCollection AddJsonPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JsonStorageOptions>(configuration.GetSection(JsonStorageOptions.SectionName));

        services.AddSingleton<IShipmentRepository, JsonShipmentRepository>();
        services.AddSingleton<IOrderRepository, JsonOrderRepository>();
        services.AddSingleton<IHubRepository, JsonHubRepository>();
        services.AddSingleton<IZoneRepository, JsonZoneRepository>();
        services.AddSingleton<IRoadRepository, JsonRoadRepository>();
        services.AddSingleton<IUserRepository, JsonUserRepository>();

        return services;
    }

    private static IServiceCollection AddMongoPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbOptions>(configuration.GetSection(MongoDbOptions.SectionName));
        services.AddSingleton<MongoDbContext>();

        services.AddSingleton<IShipmentRepository, MongoShipmentRepository>();
        services.AddSingleton<IOrderRepository, MongoOrderRepository>();
        services.AddSingleton<IHubRepository, MongoHubRepository>();
        services.AddSingleton<IZoneRepository, MongoZoneRepository>();
        services.AddSingleton<IRoadRepository, MongoRoadRepository>();
        services.AddSingleton<IUserRepository, MongoUserRepository>();

        return services;
    }
}
