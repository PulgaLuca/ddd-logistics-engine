using System.Text.Json;

namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>
/// Base riutilizzabile (DRY) per i repository su file JSON: incapsula lock,
/// lettura e scrittura di una lista di documenti. I repository concreti
/// forniscono solo il path e la mappatura documento &lt;-&gt; dominio.
/// </summary>
/// <typeparam name="TDocument">Modello di persistenza serializzato.</typeparam>
internal abstract class JsonEntityRepository<TDocument> : IDisposable
{
    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = true,
    };

    private readonly SemaphoreSlim _fileLock = new(1, 1);
    private readonly string _filePath;

    protected JsonEntityRepository(string filePath)
    {
        _filePath = filePath;
    }

    /// <summary>Legge lo store sotto lock e proietta un risultato.</summary>
    protected async Task<TResult> QueryAsync<TResult>(Func<List<TDocument>, TResult> query, CancellationToken cancellationToken)
    {
        await _fileLock.WaitAsync(cancellationToken);
        try
        {
            return query(await ReadDocumentsAsync(cancellationToken));
        }
        finally
        {
            _fileLock.Release();
        }
    }

    /// <summary>Legge, muta e riscrive lo store atomicamente (sotto lock).</summary>
    protected async Task MutateAsync(Action<List<TDocument>> mutate, CancellationToken cancellationToken)
    {
        await _fileLock.WaitAsync(cancellationToken);
        try
        {
            List<TDocument> documents = await ReadDocumentsAsync(cancellationToken);
            mutate(documents);
            await WriteDocumentsAsync(documents, cancellationToken);
        }
        finally
        {
            _fileLock.Release();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _fileLock.Dispose();
        }
    }

    private async Task<List<TDocument>> ReadDocumentsAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_filePath))
        {
            return [];
        }

        await using FileStream stream = File.OpenRead(_filePath);

        if (stream.Length == 0)
        {
            return [];
        }

        return await JsonSerializer.DeserializeAsync<List<TDocument>>(
            stream, _serializerOptions, cancellationToken) ?? [];
    }

    private async Task WriteDocumentsAsync(List<TDocument> documents, CancellationToken cancellationToken)
    {
        string? directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using FileStream stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, documents, _serializerOptions, cancellationToken);
    }
}
