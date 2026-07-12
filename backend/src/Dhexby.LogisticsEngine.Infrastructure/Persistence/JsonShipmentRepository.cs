using System.Text.Json;
using Dhexby.LogisticsEngine.Domain.Shipments;
using Microsoft.Extensions.Options;

namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>
/// Implementazione concreta del repository su file JSON (registrata come
/// singleton; il semaforo serializza gli accessi concorrenti al file).
/// </summary>
public sealed class JsonShipmentRepository : IShipmentRepository, IDisposable
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
    };

    private readonly SemaphoreSlim _fileLock = new(1, 1);
    private readonly string _filePath;

    public JsonShipmentRepository(IOptions<ShipmentStorageOptions> options)
    {
        _filePath = options.Value.ShipmentsFilePath;
    }

    public async Task<Shipment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _fileLock.WaitAsync(cancellationToken);
        try
        {
            List<ShipmentDocument> documents = await ReadDocumentsAsync(cancellationToken);

            return documents.Find(document => document.Id == id)?.ToDomain();
        }
        finally
        {
            _fileLock.Release();
        }
    }

    public async Task<bool> ExistsByTrackingNumberAsync(
        TrackingNumber trackingNumber,
        CancellationToken cancellationToken = default)
    {
        await _fileLock.WaitAsync(cancellationToken);
        try
        {
            List<ShipmentDocument> documents = await ReadDocumentsAsync(cancellationToken);

            return documents.Exists(document => document.TrackingNumber == trackingNumber.Value);
        }
        finally
        {
            _fileLock.Release();
        }
    }

    public async Task AddAsync(Shipment shipment, CancellationToken cancellationToken = default)
    {
        await _fileLock.WaitAsync(cancellationToken);
        try
        {
            List<ShipmentDocument> documents = await ReadDocumentsAsync(cancellationToken);

            documents.Add(ShipmentDocument.FromDomain(shipment));

            await WriteDocumentsAsync(documents, cancellationToken);
        }
        finally
        {
            _fileLock.Release();
        }
    }

    public async Task UpdateAsync(Shipment shipment, CancellationToken cancellationToken = default)
    {
        await _fileLock.WaitAsync(cancellationToken);
        try
        {
            List<ShipmentDocument> documents = await ReadDocumentsAsync(cancellationToken);

            int index = documents.FindIndex(document => document.Id == shipment.Id);
            if (index < 0)
            {
                throw new InvalidOperationException(
                    $"Cannot update shipment '{shipment.Id}': not found in the JSON store.");
            }

            documents[index] = ShipmentDocument.FromDomain(shipment);

            await WriteDocumentsAsync(documents, cancellationToken);
        }
        finally
        {
            _fileLock.Release();
        }
    }

    public void Dispose() => _fileLock.Dispose();

    private async Task<List<ShipmentDocument>> ReadDocumentsAsync(CancellationToken cancellationToken)
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

        return await JsonSerializer.DeserializeAsync<List<ShipmentDocument>>(
            stream, SerializerOptions, cancellationToken) ?? [];
    }

    private async Task WriteDocumentsAsync(List<ShipmentDocument> documents, CancellationToken cancellationToken)
    {
        string? directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using FileStream stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, documents, SerializerOptions, cancellationToken);
    }
}
