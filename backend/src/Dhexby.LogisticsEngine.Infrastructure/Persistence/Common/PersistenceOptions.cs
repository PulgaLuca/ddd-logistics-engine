namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>
/// Selettore del provider di persistenza (sezione "Persistence").
/// Valori supportati: "Json" (default), "Mongo" e "Postgres".
/// </summary>
public sealed class PersistenceOptions
{
    public const string SectionName = "Persistence";

    public const string JsonProvider = "Json";
    public const string MongoProvider = "Mongo";
    public const string PostgresProvider = "Postgres";

    public string Provider { get; set; } = JsonProvider;
}
