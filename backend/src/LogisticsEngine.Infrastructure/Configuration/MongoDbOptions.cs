namespace LogisticsEngine.Infrastructure.Configuration;

/// <summary>Configurazione minima per collegare Infrastructure a MongoDB Atlas.</summary>
public sealed record MongoDbOptions(string ConnectionString, string DatabaseName)
{
    public static MongoDbOptions FromEnvironment()
    {
        // La URI include host e username; la password separata serve per non scriverla nell'URI d'esempio.
        string connectionString = Environment.GetEnvironmentVariable("MONGODB_URI") ?? string.Empty;
        string password = Environment.GetEnvironmentVariable("MONGODB_PASSWORD") ?? string.Empty;
        // Un database separato per ambiente semplifica l'isolamento dei dati di sviluppo e produzione.
        string databaseName = Environment.GetEnvironmentVariable("MONGODB_DATABASE") ?? "logisticsengine";

        if (connectionString.Contains("<db_password>", StringComparison.Ordinal))
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new InvalidOperationException(
                    "MONGODB_PASSWORD è obbligatoria quando MONGODB_URI contiene <db_password>.");
            }

            // I caratteri riservati (per esempio @ o /) vanno codificati quando la password è parte della URI.
            connectionString = connectionString.Replace(
                "<db_password>", Uri.EscapeDataString(password), StringComparison.Ordinal);
        }

        // Blocca i segnaposto rimasti: un URI di esempio non deve avviare tentativi di connessione ambigui.
        if (string.IsNullOrWhiteSpace(connectionString) ||
            connectionString.Contains('<') || connectionString.Contains('>'))
        {
            throw new InvalidOperationException(
                "Configura MONGODB_URI con la connection string Atlas reale nel file .env o nell'ambiente.");
        }

        if (string.IsNullOrWhiteSpace(databaseName))
        {
            throw new InvalidOperationException("MONGODB_DATABASE non può essere vuota.");
        }

        return new MongoDbOptions(connectionString, databaseName);
    }
}
