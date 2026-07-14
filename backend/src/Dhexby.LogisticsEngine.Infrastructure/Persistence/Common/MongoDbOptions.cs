namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>Opzioni di connessione a MongoDB (sezione "Mongo").</summary>
public sealed class MongoDbOptions
{
    public const string SectionName = "Mongo";

    public string ConnectionString { get; set; } = string.Empty;

    public string Database { get; set; } = "LogisticsEngine";
}
