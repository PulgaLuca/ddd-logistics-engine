using System.Text.Json;
using LogisticsEngine.Domain.Hubs;
using LogisticsEngine.Domain.Hubs.Entities;
using LogisticsEngine.Domain.Shared.Names;
using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Infrastructure.Hubs;

/// <summary>Persistenza JSON degli Hub in un unico documento hubs.json.</summary>
public sealed class JsonHubRepository(string dataDirectory) : IHubRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private readonly string _directory = Path.GetFullPath(dataDirectory);
    private string FilePath => Path.Combine(_directory, "hubs.json");

    public Hub? GetById(Guid id) => ReadAll().FirstOrDefault(hub => hub.Id == id);

    public Hub? GetByName(Name name) => ReadAll().FirstOrDefault(hub =>
        string.Equals(hub.Name.Value, name.Value, StringComparison.OrdinalIgnoreCase));

    public IReadOnlyList<Hub> GetAll() => ReadAll()
        .OrderBy(hub => hub.Name.Value, StringComparer.Ordinal)
        .ToArray();

    public void Add(Hub hub)
    {
        List<HubDocument> documents = ReadDocuments();
        if (documents.Any(document => document.Id == hub.Id))
        {
            throw new InvalidOperationException($"Hub '{hub.Id}' already exists.");
        }

        documents.Add(ToDocument(hub));
        WriteAll(documents);
    }

    public bool Update(Hub hub)
    {
        List<HubDocument> documents = ReadDocuments();
        int index = documents.FindIndex(document => document.Id == hub.Id);
        if (index < 0)
        {
            return false;
        }

        documents[index] = ToDocument(hub);
        WriteAll(documents);
        return true;
    }

    public bool Delete(Guid id)
    {
        List<HubDocument> documents = ReadDocuments();
        int removed = documents.RemoveAll(document => document.Id == id);
        if (removed == 0)
        {
            return false;
        }

        WriteAll(documents);
        return true;
    }

    private List<Hub> ReadAll() => ReadDocuments().Select(ToEntity).ToList();

    private List<HubDocument> ReadDocuments()
    {
        if (!File.Exists(FilePath))
        {
            return [];
        }

        HubDocument[] documents = JsonSerializer.Deserialize<HubDocument[]>(File.ReadAllText(FilePath), JsonOptions)
            ?? throw new InvalidDataException($"Empty hub document: {FilePath}");
        if (documents.Any(document => document is null))
        {
            throw new InvalidDataException($"Invalid hub document: {FilePath}");
        }

        return documents.ToList();
    }

    private static Hub ToEntity(HubDocument document)
    {
        Result<Hub> result = Hub.Create(document.Id, document.Name, document.Zone);
        return result.IsSuccess
            ? result.Value
            : throw new InvalidDataException($"Invalid hub data for '{document.Id}': {result.Error.Code}");
    }

    private static HubDocument ToDocument(Hub hub) => new(hub.Id, hub.Name.Value, hub.Zone.Code);

    private void WriteAll(IReadOnlyCollection<HubDocument> documents)
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

    private sealed record HubDocument(Guid Id, string Name, string Zone);
}
