using System.Text.Json;
using LogisticsEngine.Domain.Hubs;
using LogisticsEngine.Domain.Hubs.Errors;
using LogisticsEngine.Domain.Hubs.Vo;

namespace LogisticsEngine.Infrastructure.Hubs;

/// <summary>Salva gli archi del grafo Hub nel documento hub-connections.json.</summary>
public sealed class JsonHubConnectionRepository(string dataDirectory) : IHubConnectionRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private readonly string _directory = Path.GetFullPath(dataDirectory);
    private string FilePath => Path.Combine(_directory, "hub-connections.json");

    public IReadOnlyList<HubConnection> GetAll()
    {
        if (!File.Exists(FilePath))
        {
            return [];
        }

        ConnectionDocument[] documents = JsonSerializer.Deserialize<ConnectionDocument[]>(File.ReadAllText(FilePath), JsonOptions)
            ?? throw new InvalidDataException($"Empty connection document: {FilePath}");
        return documents.Select(ToConnection).ToArray();
    }

    public void Add(HubConnection connection)
    {
        List<ConnectionDocument> documents = ReadDocuments();
        bool alreadyExists = documents.Any(document =>
            (document.FirstHubId == connection.FirstHubId && document.SecondHubId == connection.SecondHubId)
            || (document.FirstHubId == connection.SecondHubId && document.SecondHubId == connection.FirstHubId));
        if (alreadyExists)
        {
            throw new InvalidOperationException("A connection between these Hubs already exists.");
        }

        documents.Add(ToDocument(connection));
        WriteAll(documents);
    }

    private List<ConnectionDocument> ReadDocuments()
    {
        if (!File.Exists(FilePath))
        {
            return [];
        }

        ConnectionDocument[] documents = JsonSerializer.Deserialize<ConnectionDocument[]>(File.ReadAllText(FilePath), JsonOptions)
            ?? throw new InvalidDataException($"Empty connection document: {FilePath}");
        return documents.ToList();
    }

    private static HubConnection ToConnection(ConnectionDocument document)
    {
        var result = HubConnection.Create(document.FirstHubId, document.SecondHubId, document.Distance);
        return result.IsSuccess
            ? result.Value
            : throw new InvalidDataException($"Invalid Hub connection: {HubErrors.InvalidConnection.Code}");
    }

    private static ConnectionDocument ToDocument(HubConnection connection) =>
        new(connection.FirstHubId, connection.SecondHubId, connection.Distance);

    private void WriteAll(IReadOnlyCollection<ConnectionDocument> documents)
    {
        Directory.CreateDirectory(_directory);
        string temporaryPath = FilePath + ".tmp";
        try
        {
            File.WriteAllText(temporaryPath, JsonSerializer.Serialize(documents, JsonOptions));
            File.Move(temporaryPath, FilePath, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
    }

    private sealed record ConnectionDocument(Guid FirstHubId, Guid SecondHubId, decimal Distance);
}
