using LogisticsEngine.Domain.Hubs;
using LogisticsEngine.Domain.Hubs.Vo;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace LogisticsEngine.Infrastructure.Hubs;

/// <summary>Persiste archi non diretti nella collezione hubConnections.</summary>
public sealed class MongoHubConnectionRepository(IMongoDatabase database) : IHubConnectionRepository
{
    // La collezione contiene un documento per ogni arco non orientato del grafo degli Hub.
    private readonly IMongoCollection<HubConnectionDocument> _connections =
        database.GetCollection<HubConnectionDocument>("hubConnections");

    public IReadOnlyList<HubConnection> GetAll() => _connections
        .Find(Builders<HubConnectionDocument>.Filter.Empty)
        .ToList()
        .Select(ToValueObject)
        .ToArray();

    public void Add(HubConnection connection) => _connections.InsertOne(ToDocument(connection));

    private static HubConnection ToValueObject(HubConnectionDocument document)
    {
        // Il costruttore del value object valida distanze e ID e ordina gli estremi dell'arco.
        var result = HubConnection.Create(document.FirstHubId, document.SecondHubId, document.Distance);
        return result.IsSuccess
            ? result.Value
            : throw new InvalidDataException($"Invalid hub connection document '{document.Id}'.");
    }

    private static HubConnectionDocument ToDocument(HubConnection connection) => new()
    {
        // HubConnection ordina gli endpoint alla creazione: l'ID composto identifica lo stesso arco in entrambe le direzioni.
        Id = $"{connection.FirstHubId:N}-{connection.SecondHubId:N}",
        FirstHubId = connection.FirstHubId,
        SecondHubId = connection.SecondHubId,
        Distance = connection.Distance
    };

    public sealed class HubConnectionDocument
    {
        public HubConnectionDocument() { }
        [BsonId] public string Id { get; set; } = string.Empty;
        [BsonElement("firstHubId")]
        // UUID subtype 4 evita ambiguità di byte-order quando il GUID viene letto da altri driver.
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid FirstHubId { get; set; }
        [BsonElement("secondHubId")]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid SecondHubId { get; set; }
        [BsonElement("distance")] public decimal Distance { get; set; }
    }
}
