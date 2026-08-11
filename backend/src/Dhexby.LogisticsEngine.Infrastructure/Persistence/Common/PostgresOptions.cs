namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>
/// Opzioni di connessione a PostgreSQL (sezione "Postgres").
/// In Docker vengono sovrascritte dalla variabile d'ambiente
/// <c>Postgres__ConnectionString</c>.
/// </summary>
public sealed class PostgresOptions
{
    public const string SectionName = "Postgres";

    /// <summary>Connection string ADO.NET (formato Npgsql).</summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Applica automaticamente le migration all'avvio dell'applicazione.
    /// Comodo in sviluppo e con Docker Compose; in produzione si preferisce
    /// un passo di deploy dedicato.
    /// </summary>
    public bool MigrateOnStartup { get; set; } = true;

    /// <summary>
    /// Tentativi di connessione al database durante la migrazione di avvio,
    /// per tollerare un database non ancora pronto ad accettare connessioni.
    /// </summary>
    public int MaxRetryCount { get; set; } = 5;
}
