using Dhexby.LogisticsEngine.Application.Abstractions.Authentication;
using Dhexby.LogisticsEngine.Application.Abstractions.Persistence;
using Dhexby.LogisticsEngine.Domain.Hubs;
using Dhexby.LogisticsEngine.Domain.Orders;
using Dhexby.LogisticsEngine.Domain.Routing;
using Dhexby.LogisticsEngine.Domain.Shipments;
using Dhexby.LogisticsEngine.Domain.Users;
using Dhexby.LogisticsEngine.Domain.Zones;
using Dhexby.LogisticsEngine.Infrastructure.Authentication;
using Dhexby.LogisticsEngine.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dhexby.LogisticsEngine.Infrastructure;

/// <summary>
/// Composition root del layer Infrastructure: lega i contratti del dominio
/// alle implementazioni concrete. Il provider di persistenza (JSON, MongoDB o
/// PostgreSQL) è scelto in base alla configurazione: cambiando
/// "Persistence:Provider" si sostituisce l'intero storage senza toccare
/// Application, Domain o Api.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        // Servizi di autenticazione, indipendenti dal provider di persistenza.
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        string? provider = configuration.GetSection(PersistenceOptions.SectionName)["Provider"];

        return provider switch
        {
            not null when Is(provider, PersistenceOptions.PostgresProvider) =>
                services.AddPostgresPersistence(configuration),
            not null when Is(provider, PersistenceOptions.MongoProvider) =>
                services.AddMongoPersistence(configuration),
            _ => services.AddJsonPersistence(configuration),
        };

        static bool Is(string value, string expected) =>
            string.Equals(value, expected, StringComparison.OrdinalIgnoreCase);
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

        // Store non transazionale: le scritture sono già immediate.
        services.AddSingleton<IUnitOfWork, NoOpUnitOfWork>();

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

        services.AddSingleton<IUnitOfWork, NoOpUnitOfWork>();

        return services;
    }

    /// <summary>
    /// Provider PostgreSQL: unico dei tre a offrire transazioni ACID reali,
    /// quindi l'unico con un Unit of Work "vero". Repository e DbContext sono
    /// Scoped perché il change tracker deve vivere quanto la singola richiesta.
    /// </summary>
    private static IServiceCollection AddPostgresPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        IConfigurationSection section = configuration.GetSection(PostgresOptions.SectionName);
        services.Configure<PostgresOptions>(section);

        var options = section.Get<PostgresOptions>() ?? new PostgresOptions();

        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            throw new InvalidOperationException(
                "Persistence:Provider è 'Postgres' ma manca Postgres:ConnectionString " +
                "(in Docker: variabile d'ambiente Postgres__ConnectionString).");
        }

        // Nessun EnableRetryOnFailure: la retrying execution strategy di Npgsql
        // è incompatibile con le transazioni aperte esplicitamente dalla
        // pipeline (rieseguirebbe l'handler, e con esso gli effetti in memoria).
        // La resilienza all'avvio è gestita in DatabaseInitializer, dove il
        // retry è sicuro perché l'unica operazione è la migrazione.
        services.AddDbContext<LogisticsDbContext>(builder =>
            builder.UseNpgsql(options.ConnectionString, npgsql =>
                npgsql.MigrationsHistoryTable("__ef_migrations_history", LogisticsDbContext.Schema)));

        services.AddScoped<IShipmentRepository, PostgresShipmentRepository>();
        services.AddScoped<IOrderRepository, PostgresOrderRepository>();
        services.AddScoped<IHubRepository, PostgresHubRepository>();
        services.AddScoped<IZoneRepository, PostgresZoneRepository>();
        services.AddScoped<IRoadRepository, PostgresRoadRepository>();
        services.AddScoped<IUserRepository, PostgresUserRepository>();

        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        return services;
    }
}
