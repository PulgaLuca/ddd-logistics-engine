namespace LogisticsEngine.Infrastructure.Configuration;

/// <summary>Carica un file .env senza sovrascrivere variabili già impostate nell'ambiente.</summary>
public static class DotEnvFile
{
    public static void LoadFromRepositoryAncestors()
    {
        // Cerca prima dalla cartella di avvio, poi dalla cartella dell'app compilata: così funziona sia da CLI sia da IDE.
        string? envFile = FindEnvFile(Directory.GetCurrentDirectory())
            ?? FindEnvFile(AppContext.BaseDirectory);
        if (envFile is null)
        {
            return;
        }

        foreach (string line in File.ReadLines(envFile))
        {
            string entry = line.Trim();
            // Le righe vuote e i commenti non rappresentano variabili da caricare.
            if (entry.Length == 0 || entry.StartsWith('#'))
            {
                continue;
            }

            // Si divide solo sul primo '=' perché una URI Mongo può contenerne altri nella query string.
            int separator = entry.IndexOf('=');
            if (separator <= 0)
            {
                continue;
            }

            string key = entry[..separator].Trim();
            string value = RemoveMatchingQuotes(entry[(separator + 1)..].Trim());
            // Le variabili impostate dal sistema o dal container hanno precedenza sul file locale.
            if (!string.IsNullOrWhiteSpace(key) && Environment.GetEnvironmentVariable(key) is null)
            {
                Environment.SetEnvironmentVariable(key, value);
            }
        }
    }

    private static string? FindEnvFile(string startingDirectory)
    {
        // Risale le cartelle per trovare il file anche se l'app parte dalla directory backend/src/.../bin.
        DirectoryInfo? directory = new(startingDirectory);
        while (directory is not null)
        {
            string candidate = Path.Combine(directory.FullName, ".env");
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        return null;
    }

    // Le virgolette sono sintassi del file .env e non fanno parte del valore passato al driver.
    private static string RemoveMatchingQuotes(string value) =>
        value.Length >= 2 && value[0] == value[^1] && value[0] is '\'' or '"'
            ? value[1..^1]
            : value;
}
