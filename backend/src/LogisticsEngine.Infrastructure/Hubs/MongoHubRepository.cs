using LogisticsEngine.Domain.Hubs;
using LogisticsEngine.Domain.Hubs.Entities;
using LogisticsEngine.Domain.Shared.Names;
using LogisticsEngine.Infrastructure.Mongo;
using LogisticsEngine.SharedKernel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace LogisticsEngine.Infrastructure.Hubs;

/// <summary>Implementazione MongoDB del repository Hub; ogni Hub è un documento nella collezione hubs.</summary>
public sealed class MongoHubRepository(IMongoDatabase database) : IHubRepository
{
    // MongoClient/Database sono condivisi via DI; qui conserviamo solo il riferimento tipizzato alla collezione.
    private readonly IMongoCollection<HubDocument> _hubs = database.GetCollection<HubDocument>("hubs");

    public Hub? GetById(Guid id)
    {
        // Il filtro considera sia l'ID canonico Mongo sia il campo Id dei documenti importati in precedenza.
        HubDocument? document = _hubs.Find(ByDomainId(id)).FirstOrDefault();
        return document is null ? null : ToEntity(document);
    }

    public Hub? GetByName(Name name)
    {
        // I documenti nuovi usano `name`; quelli importati da JSON possono avere `Name` maiuscolo.
        FilterDefinition<HubDocument> filter = Builders<HubDocument>.Filter.Or(
            Builders<HubDocument>.Filter.Eq("name", name.Value),
            Builders<HubDocument>.Filter.Eq("Name", name.Value));
        HubDocument? document = _hubs.Find(filter).FirstOrDefault();
        return document is null ? null : ToEntity(document);
    }

    // Convertiamo prima in entità: così il mapping può usare i campi legacy e l'ordinamento usa sempre il nome valido.
    public IReadOnlyList<Hub> GetAll() => _hubs.Find(Builders<HubDocument>.Filter.Empty)
        .ToList()
        .Select(ToEntity)
        .OrderBy(hub => hub.Name.Value, StringComparer.Ordinal)
        .ToArray();

    public void Add(Hub hub) => _hubs.InsertOne(ToDocument(hub));

    public bool Update(Hub hub)
    {
        // $set modifica i dati senza sostituire `_id`, che MongoDB considera immutabile.
        // $unset rimuove i campi legacy maiuscoli dopo aver scritto i campi canonici in minuscolo.
        UpdateDefinition<HubDocument> update = Builders<HubDocument>.Update
            .Set(document => document.Name, hub.Name.Value)
            .Set(document => document.Zone, hub.Zone.Code)
            .Unset("Name")
            .Unset("Zone");
        UpdateResult result = _hubs.UpdateOne(ByDomainId(hub.Id), update);
        return result.MatchedCount > 0;
    }

    public bool Delete(Guid id) => _hubs.DeleteOne(ByDomainId(id)).DeletedCount > 0;

    private static Hub ToEntity(HubDocument document)
    {
        // La persistenza non sostituisce le invarianti: ogni record letto viene ricostruito tramite il factory del Domain.
        Guid id = MongoEntityId.FromBsonValue(GetDomainId(document));
        string name = ReadString(document.Name, document.ExtraElements, "Name");
        string zone = ReadString(document.Zone, document.ExtraElements, "Zone");
        Result<Hub> result = Hub.Create(id, name, zone);
        return result.IsSuccess
            ? result.Value
            : throw new InvalidDataException($"Invalid hub document '{id}': {result.Error.Code}");
    }

    private static HubDocument ToDocument(Hub hub) => new()
    {
        // I nuovi documenti hanno uno schema esplicito e stabile; non serializziamo direttamente l'entità Domain.
        Id = MongoEntityId.ToBsonValue(hub.Id),
        Name = hub.Name.Value,
        Zone = hub.Zone.Code
    };

    private static FilterDefinition<HubDocument> ByDomainId(Guid id)
    {
        BsonValue bsonId = MongoEntityId.ToBsonValue(id);
        // I dati storici possono contenere GUID BSON, ObjectId o GUID testuali sotto il campo `Id`.
        return Builders<HubDocument>.Filter.Or(
            Builders<HubDocument>.Filter.Eq(document => document.Id, bsonId),
            Builders<HubDocument>.Filter.Eq("Id", bsonId),
            Builders<HubDocument>.Filter.Eq("Id", new BsonString(id.ToString("D"))),
            Builders<HubDocument>.Filter.Eq("Id", new BsonString(id.ToString("D").ToUpperInvariant())));
    }

    private static BsonValue GetDomainId(HubDocument document)
    {
        // Se l'importazione ha conservato l'ID Domain originale, lo preferiamo all'ObjectId generato da MongoDB.
        if (document.ExtraElements is not null &&
            document.ExtraElements.TryGetValue("Id", out BsonValue legacyId) &&
            legacyId.BsonType is BsonType.Binary or BsonType.ObjectId)
        {
            return legacyId;
        }

        if (document.ExtraElements is not null &&
            document.ExtraElements.TryGetValue("Id", out legacyId) &&
            legacyId.BsonType == BsonType.String && Guid.TryParse(legacyId.AsString, out _))
        {
            return legacyId;
        }

        return document.Id;
    }

    private static string ReadString(string currentValue, BsonDocument? extraElements, string legacyFieldName)
    {
        // Il mapper standard legge `name`/`zone`; se sono assenti, recuperiamo la variante legacy senza distinzione di maiuscole.
        if (!string.IsNullOrWhiteSpace(currentValue))
        {
            return currentValue;
        }

        if (extraElements is not null)
        {
            foreach (BsonElement element in extraElements.Elements)
            {
                if (string.Equals(element.Name, legacyFieldName, StringComparison.OrdinalIgnoreCase) &&
                    element.Value.BsonType == BsonType.String)
                {
                    return element.Value.AsString;
                }
            }
        }

        return currentValue;
    }

    public sealed class HubDocument
    {
        public HubDocument() { }
        // BsonValue consente di leggere collezioni miste: ObjectId legacy e GUID BSON dei nuovi record.
        [BsonId]
        public BsonValue Id { get; set; } = BsonNull.Value;
        [BsonElement("name")] public string Name { get; set; } = string.Empty;
        [BsonElement("zone")] public string Zone { get; set; } = string.Empty;
        // Conserva temporaneamente gli altri campi importati, ad esempio `Id`, `Name` e `Zone` maiuscoli.
        [BsonExtraElements] public BsonDocument? ExtraElements { get; set; }
    }
}
