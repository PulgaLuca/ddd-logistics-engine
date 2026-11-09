using LogisticsEngine.Domain.Shipments;
using LogisticsEngine.Domain.Shipments.Entities;
using LogisticsEngine.Domain.Shipments.Enums;
using LogisticsEngine.SharedKernel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace LogisticsEngine.Infrastructure.Shipments;

/// <summary>Implementazione MongoDB del repository Shipment nella collezione shipments.</summary>
public sealed class MongoShipmentRepository(IMongoDatabase database) : IShipmentRepository
{
    // Il nome della collezione fa parte dello schema persistente e viene condiviso con la documentazione Atlas.
    private readonly IMongoCollection<ShipmentDocument> _shipments = database.GetCollection<ShipmentDocument>("shipments");

    public Shipment? GetById(Guid id)
    {
        ShipmentDocument? document = _shipments.Find(shipment => shipment.Id == id).FirstOrDefault();
        return document is null ? null : ToEntity(document);
    }

    public IReadOnlyList<Shipment> GetAll() => _shipments
        .Find(Builders<ShipmentDocument>.Filter.Empty)
        .SortBy(shipment => shipment.TrackingNumber)
        .ToList()
        .Select(ToEntity)
        .ToArray();

    public void Add(Shipment shipment) => _shipments.InsertOne(ToDocument(shipment));

    public bool Update(Shipment shipment)
    {
        ReplaceOneResult result = _shipments.ReplaceOne(
            document => document.Id == shipment.Id, ToDocument(shipment));
        return result.MatchedCount > 0;
    }

    public bool Delete(Guid id) => _shipments.DeleteOne(shipment => shipment.Id == id).DeletedCount > 0;

    private static Shipment ToEntity(ShipmentDocument document)
    {
        // Prima ricrea gli invarianti di ID e tracking number, poi ripristina lo stato usando solo transizioni di Domain valide.
        Result<Shipment> created = Shipment.Create(document.Id, document.TrackingNumber);
        if (created.IsFailure)
        {
            throw new InvalidDataException($"Invalid shipment document '{document.Id}': {created.Error.Code}");
        }

        Shipment shipment = created.Value;
        if (document.Status == ShipmentStatus.Created)
        {
            return shipment;
        }

        ShipmentStatus firstTransition = document.Status == ShipmentStatus.Delivered
            ? ShipmentStatus.InTransit
            : document.Status;
        // Delivered è raggiungibile solo passando da InTransit: ricostruiamo quel percorso invece di assegnare lo stato.
        Result firstChange = shipment.ChangeStatus(firstTransition);
        if (firstChange.IsFailure || document.Status != ShipmentStatus.Delivered)
        {
            return firstChange.IsSuccess
                ? shipment
                : throw new InvalidDataException($"Invalid shipment status for '{document.Id}'.");
        }

        return shipment.ChangeStatus(ShipmentStatus.Delivered).IsSuccess
            ? shipment
            : throw new InvalidDataException($"Invalid shipment status for '{document.Id}'.");
    }

    private static ShipmentDocument ToDocument(Shipment shipment) => new()
    {
        // Salviamo i valori semplici del modello persistente, non le classi interne del Domain.
        // Il GUID usa il formato BSON standard per rimanere compatibile con client non .NET.
        Id = shipment.Id,
        TrackingNumber = shipment.TrackingNumber.Value,
        Status = shipment.Status
    };

    public sealed class ShipmentDocument
    {
        public ShipmentDocument() { }
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid Id { get; set; }
        [BsonElement("trackingNumber")] public string TrackingNumber { get; set; } = string.Empty;
        [BsonElement("status")] public ShipmentStatus Status { get; set; }
    }
}
