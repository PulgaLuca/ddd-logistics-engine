using Dhexby.LogisticsEngine.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Dhexby.LogisticsEngine.Infrastructure;

/// <summary>
/// Applica le migration EF Core all'avvio del processo. Con Docker Compose il
/// container dell'API parte insieme a quello del database: questo passo evita
/// di dover eseguire manualmente <c>dotnet ef database update</c> ad ogni
/// <c>docker compose up</c>.
/// </summary>
public static class DatabaseInitializer
{
    /// <summary>
    /// Migra il database se il provider attivo è PostgreSQL e
    /// <see cref="PostgresOptions.MigrateOnStartup"/> è abilitato.
    /// Con gli altri provider non fa nulla.
    /// </summary>
    public static async Task MigrateDatabaseAsync(
        this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(services);

        await using AsyncServiceScope scope = services.CreateAsyncScope();

        LogisticsDbContext? context = scope.ServiceProvider.GetService<LogisticsDbContext>();
        if (context is null)
        {
            return;
        }

        PostgresOptions options = scope.ServiceProvider.GetRequiredService<IOptions<PostgresOptions>>().Value;
        if (!options.MigrateOnStartup)
        {
            return;
        }

        ILogger logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger(typeof(DatabaseInitializer));

        MigrationLog.Applying(logger);

        await MigrateWithRetryAsync(context, options.MaxRetryCount, logger, cancellationToken);

        MigrationLog.Applied(logger);
    }

    /// <summary>
    /// Ritenta la migrazione finché il database non accetta connessioni: con
    /// Docker Compose il container dell'API può essere pronto prima che
    /// PostgreSQL abbia completato l'inizializzazione. Il retry è sicuro perché
    /// applicare le migration è idempotente.
    /// </summary>
    private static async Task MigrateWithRetryAsync(
        LogisticsDbContext context,
        int maxRetryCount,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        for (int attempt = 1; ; attempt++)
        {
            try
            {
                await context.Database.MigrateAsync(cancellationToken);
                return;
            }
            catch (NpgsqlException exception) when (attempt <= maxRetryCount)
            {
                MigrationLog.Retrying(logger, attempt, maxRetryCount, exception.Message);
                await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
            }
        }
    }
}

/// <summary>Messaggi di log generati a compile time (LoggerMessage source generator).</summary>
internal static partial class MigrationLog
{
    [LoggerMessage(
        EventId = 2000,
        Level = LogLevel.Information,
        Message = "Applicazione delle migration EF Core su PostgreSQL...")]
    public static partial void Applying(ILogger logger);

    [LoggerMessage(
        EventId = 2001,
        Level = LogLevel.Information,
        Message = "Schema del database allineato.")]
    public static partial void Applied(ILogger logger);

    [LoggerMessage(
        EventId = 2002,
        Level = LogLevel.Warning,
        Message = "Database non raggiungibile (tentativo {Attempt}/{MaxRetryCount}): {Reason}")]
    public static partial void Retrying(ILogger logger, int attempt, int maxRetryCount, string reason);
}
