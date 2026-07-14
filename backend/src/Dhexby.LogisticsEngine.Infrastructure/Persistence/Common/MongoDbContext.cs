using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>
/// Contesto MongoDB: apre la connessione al database "LogisticsEngine" e
/// fornisce le collection tipizzate ai repository. Una collection per aggregate.
/// </summary>
internal sealed class MongoDbContext
{
    public const string ShipmentsCollection = "shipments";
    public const string OrdersCollection = "orders";
    public const string HubsCollection = "hubs";
    public const string ZonesCollection = "zones";
    public const string RoadsCollection = "roads";

    private readonly IMongoDatabase _database;

    static MongoDbContext()
    {
        // Rappresentazioni BSON stabili: Guid standard e decimal come Decimal128.
        BsonSerializer.TryRegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        BsonSerializer.TryRegisterSerializer(new DecimalSerializer(BsonType.Decimal128));
        BsonSerializer.TryRegisterSerializer(
            new NullableSerializer<decimal>(new DecimalSerializer(BsonType.Decimal128)));
    }

    public MongoDbContext(IOptions<MongoDbOptions> options)
    {
        MongoDbOptions value = options.Value;
        var client = new MongoClient(value.ConnectionString);
        _database = client.GetDatabase(value.Database);
    }

    public IMongoCollection<TDocument> GetCollection<TDocument>(string name) =>
        _database.GetCollection<TDocument>(name);
}
