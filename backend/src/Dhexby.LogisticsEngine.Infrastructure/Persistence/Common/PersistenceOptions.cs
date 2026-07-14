namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>
/// Selettore del provider di persistenza (sezione "Persistence").
/// Valori supportati: "Json" (default) e "Mongo".
/// </summary>
public sealed class PersistenceOptions
{
    public const string SectionName = "Persistence";

    public const string JsonProvider = "Json";
    public const string MongoProvider = "Mongo";

    public string Provider { get; set; } = JsonProvider;
}
