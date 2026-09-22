using System.Text.Json;
using LogisticsEngine.Domain.Hubs;
using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Infrastructure;

/// <summary>Repository didattico per un singolo processo: un file JSON per hub.</summary>
public sealed class JsonHubRepository(string dataDirectory) : IHubRepository
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };
    private readonly string _directory = Path.GetFullPath(dataDirectory);
    private readonly SemaphoreSlim _gate = new(1, 1);

    public async Task<Hub?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            return await ReadAsync(FilePath(id), cancellationToken);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<IReadOnlyList<Hub>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            if (!Directory.Exists(_directory))
            {
                return [];
            }

            var hubs = new List<Hub>();
            foreach (string path in Directory.EnumerateFiles(_directory, "*.json"))
            {
                Hub? hub = await ReadAsync(path, cancellationToken);
                if (hub is not null)
                {
                    hubs.Add(hub);
                }
            }

            return hubs.OrderBy(hub => hub.Name, StringComparer.Ordinal).ThenBy(hub => hub.Id).ToArray();
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task AddAsync(Hub hub, CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            await WriteAsync(hub, overwrite: false, cancellationToken);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<bool> UpdateAsync(Hub hub, CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            if (!File.Exists(FilePath(hub.Id)))
            {
                return false;
            }

            await WriteAsync(hub, overwrite: true, cancellationToken);
            return true;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            string path = FilePath(id);
            if (!File.Exists(path))
            {
                return false;
            }

            File.Delete(path);
            return true;
        }
        finally
        {
            _gate.Release();
        }
    }

    private string FilePath(Guid id) => Path.Combine(_directory, $"{id:D}.json");

    private static async Task<Hub?> ReadAsync(string path, CancellationToken cancellationToken)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        await using FileStream stream = File.OpenRead(path);
        HubDocument? document = await JsonSerializer.DeserializeAsync<HubDocument>(stream, _jsonOptions, cancellationToken);
        if (document is null)
        {
            throw new InvalidDataException($"Empty hub document: {path}");
        }

        Result<Hub> result = Hub.Create(document.Id, document.Name, document.Zone);
        if (result.IsFailure || Path.GetFileNameWithoutExtension(path) != document.Id.ToString("D"))
        {
            throw new InvalidDataException($"Invalid hub document: {path}");
        }

        return result.Value;
    }

    private async Task WriteAsync(Hub hub, bool overwrite, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(_directory);
        string path = FilePath(hub.Id);
        string temporaryPath = path + ".tmp";
        try
        {
            var document = new HubDocument(hub.Id, hub.Name, hub.Zone.Code);
            await File.WriteAllTextAsync(temporaryPath, JsonSerializer.Serialize(document, _jsonOptions), cancellationToken);
            // La sostituzione evita di lasciare un JSON parziale in caso di scrittura interrotta.
            File.Move(temporaryPath, path, overwrite);
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
