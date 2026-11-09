using MongoDB.Bson;

namespace LogisticsEngine.Infrastructure.Mongo;

/// <summary>Converte gli ID Mongo esistenti in GUID di dominio senza perdere la possibilità di riscriverli.</summary>
internal static class MongoEntityId
{
    // Un ObjectId occupa 12 byte, mentre il Domain usa Guid da 16 byte.
    // Il marcatore negli ultimi 4 byte rende riconoscibili i Guid derivati e permette di tornare all'ObjectId originale.
    private static readonly byte[] ObjectIdMarker = [0x4C, 0x45, 0x4F, 0x49]; // "LEOI"

    // Accetta gli ID usati dal driver moderno, quelli storici ObjectId e i GUID testuali importati.
    public static Guid FromBsonValue(BsonValue value) => value.BsonType switch
    {
        BsonType.Binary => value.AsBsonBinaryData.ToGuid(GuidRepresentation.Standard),
        BsonType.ObjectId => FromObjectId(value.AsObjectId),
        BsonType.String when Guid.TryParse(value.AsString, out Guid id) => id,
        _ => throw new InvalidDataException($"Unsupported MongoDB _id BSON type: {value.BsonType}.")
    };

    public static BsonValue ToBsonValue(Guid id)
    {
        byte[] bytes = id.ToByteArray();
        // Se il GUID contiene il marcatore, è la forma reversibile di un ObjectId storico.
        if (bytes.AsSpan(12, ObjectIdMarker.Length).SequenceEqual(ObjectIdMarker))
        {
            return new BsonObjectId(new ObjectId(bytes.AsSpan(0, 12).ToArray()));
        }

        // I nuovi ID vengono salvati come UUID BSON standard, interoperabile fra driver diversi.
        return new BsonBinaryData(id, GuidRepresentation.Standard);
    }

    private static Guid FromObjectId(ObjectId objectId)
    {
        byte[] bytes = new byte[16];
        // Copia i 12 byte dell'ObjectId e completa il GUID con il marcatore riservato.
        objectId.ToByteArray().CopyTo(bytes, 0);
        ObjectIdMarker.CopyTo(bytes, 12);
        return new Guid(bytes);
    }
}
