using System.Text.Json;
using LogisticsEngine.Domain.Shipments;
using LogisticsEngine.Domain.Shipments.Entities;
using LogisticsEngine.Domain.Shipments.Enums;
using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Infrastructure.Shipments;

/// <summary>Persistenza JSON delle spedizioni in un unico documento shipments.json.</summary>
public sealed class JsonShipmentRepository(string dataDirectory) : IShipmentRepository
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };
    private readonly string _directory = Path.GetFullPath(dataDirectory);
    private string FilePath => Path.Combine(_directory, "shipments.json");

    public Shipment? GetById(Guid id) => ReadAll().FirstOrDefault(shipment => shipment.Id == id);

    public IReadOnlyList<Shipment> GetAll() => ReadAll()
        .OrderBy(shipment => shipment.TrackingNumber.Value, StringComparer.Ordinal)
        .ToArray();

    public void Add(Shipment shipment)
    {
        List<ShipmentDocument> documents = ReadDocuments();
        if (documents.Any(document => document.Id == shipment.Id))
        {
            throw new InvalidOperationException($"Shipment '{shipment.Id}' already exists.");
        }

        documents.Add(ToDocument(shipment));
        WriteAll(documents);
    }

    public bool Update(Shipment shipment)
    {
        List<ShipmentDocument> documents = ReadDocuments();
        int index = documents.FindIndex(document => document.Id == shipment.Id);
        if (index < 0)
        {
            return false;
        }

        documents[index] = ToDocument(shipment);
        WriteAll(documents);
        return true;
    }

    public bool Delete(Guid id)
    {
        List<ShipmentDocument> documents = ReadDocuments();
        int removed = documents.RemoveAll(document => document.Id == id);
        if (removed == 0)
        {
            return false;
        }

        WriteAll(documents);
        return true;
    }

    private List<Shipment> ReadAll() => ReadDocuments().Select(ToEntity).ToList();

    private List<ShipmentDocument> ReadDocuments()
    {
        if (!File.Exists(FilePath))
        {
            return [];
        }

        ShipmentDocument[] documents = JsonSerializer.Deserialize<ShipmentDocument[]>(File.ReadAllText(FilePath), _jsonOptions)
            ?? throw new InvalidDataException($"Empty shipment document: {FilePath}");
        if (documents.Any(document => document is null))
        {
            throw new InvalidDataException($"Invalid shipment document: {FilePath}");
        }

        return documents.ToList();
    }

    private static Shipment ToEntity(ShipmentDocument document)
    {
        Result<Shipment> created = Shipment.Create(document.Id, document.TrackingNumber);
        if (created.IsFailure)
        {
            throw new InvalidDataException($"Invalid shipment data for '{document.Id}': {created.Error.Code}");
        }

        Shipment shipment = created.Value;
        Result restored = RestoreStatus(shipment, document.Status);
        return restored.IsSuccess
            ? shipment
            : throw new InvalidDataException($"Invalid shipment status for '{document.Id}'.");
    }

    private static Result RestoreStatus(Shipment shipment, ShipmentStatus status)
    {
        if (status == ShipmentStatus.Created)
        {
            return Result.Success();
        }

        Result started = shipment.ChangeStatus(status == ShipmentStatus.Delivered ? ShipmentStatus.InTransit : status);
        if (started.IsFailure || status != ShipmentStatus.Delivered)
        {
            return started;
        }

        return shipment.ChangeStatus(ShipmentStatus.Delivered);
    }

    private static ShipmentDocument ToDocument(Shipment shipment) =>
        new(shipment.Id, shipment.TrackingNumber.Value, shipment.Status);

    private void WriteAll(IReadOnlyCollection<ShipmentDocument> documents)
    {
        Directory.CreateDirectory(_directory);
        string temporaryPath = FilePath + ".tmp";
        try
        {
            File.WriteAllText(temporaryPath, JsonSerializer.Serialize(documents, _jsonOptions));
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

    private sealed record ShipmentDocument(Guid Id, string TrackingNumber, ShipmentStatus Status);
}
