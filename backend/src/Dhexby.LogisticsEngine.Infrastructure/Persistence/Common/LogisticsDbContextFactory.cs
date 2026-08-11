using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>
/// Factory usata solo dai tool a design time (<c>dotnet ef migrations add</c>,
/// <c>dotnet ef database update</c>). Permette di generare le migration senza
/// avviare l'API e senza che il progetto Infrastructure debba conoscere il layer Api.
/// <para>
/// La connection string si può passare come argomento del comando ef oppure con
/// la variabile d'ambiente <c>Postgres__ConnectionString</c>.
/// </para>
/// </summary>
public sealed class LogisticsDbContextFactory : IDesignTimeDbContextFactory<LogisticsDbContext>
{
    public LogisticsDbContext CreateDbContext(string[] args)
    {
        const string fallback = "Host=localhost;Port=5432;Database=logistics;Username=postgres;Password=postgres";

        string connectionString =
            args is { Length: > 0 } ? args[0]
            : Environment.GetEnvironmentVariable("Postgres__ConnectionString") is { Length: > 0 } fromEnvironment
                ? fromEnvironment
                : fallback;

        DbContextOptions<LogisticsDbContext> options =
            new DbContextOptionsBuilder<LogisticsDbContext>()
                .UseNpgsql(connectionString)
                .Options;

        return new LogisticsDbContext(options);
    }
}
