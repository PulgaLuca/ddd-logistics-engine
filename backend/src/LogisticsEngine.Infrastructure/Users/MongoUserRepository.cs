using LogisticsEngine.Domain.Users;
using LogisticsEngine.Domain.Users.Entities;
using LogisticsEngine.Domain.Users.Enums;
using LogisticsEngine.Domain.Users.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace LogisticsEngine.Infrastructure.Users;

/// <summary>Persistenza utenti MongoDB; conserva il password hash e mai la password in chiaro.</summary>
public sealed class MongoUserRepository(IMongoDatabase database) : IUserRepository
{
    // In Mongo i campi sono valori semplici; la password salvata è sempre l'hash prodotto dal servizio PBKDF2.
    private readonly IMongoCollection<UserDocument> _users = database.GetCollection<UserDocument>("users");
    private readonly Lock _indexLock = new();
    private bool _userNameIndexCreated;

    public User? GetById(Guid id)
    {
        UserDocument? document = _users.Find(user => user.Id == id).FirstOrDefault();
        return document is null ? null : ToEntity(document);
    }

    public User? GetByUserName(UserName userName)
    {
        // L'indice rende la ricerca del login efficiente e impone la stessa unicità del nome a livello database.
        EnsureUniqueUserNameIndex();
        UserDocument? document = _users.Find(user => user.UserName == userName.Value).FirstOrDefault();
        return document is null ? null : ToEntity(document);
    }

    public void Add(User user)
    {
        EnsureUniqueUserNameIndex();
        _users.InsertOne(ToDocument(user));
    }

    private void EnsureUniqueUserNameIndex()
    {
        if (_userNameIndexCreated)
        {
            return;
        }

        lock (_indexLock)
        {
            if (_userNameIndexCreated)
            {
                return;
            }

            // Il vincolo vive nel database, quindi protegge anche registrazioni concorrenti tra più processi API.
            var index = new CreateIndexModel<UserDocument>(
                Builders<UserDocument>.IndexKeys.Ascending(user => user.UserName),
                new CreateIndexOptions { Name = "uq_users_userName", Unique = true });
            _users.Indexes.CreateOne(index);
            _userNameIndexCreated = true;
        }
    }

    private static User ToEntity(UserDocument document)
    {
        // Ricostruiamo ruolo e value object per non lasciare che dati persistiti aggirino le invarianti Domain.
        if (!Enum.TryParse(document.Role, ignoreCase: false, out UserRole role))
        {
            throw new InvalidDataException($"Invalid user role for '{document.Id}'.");
        }

        var result = User.Create(document.Id, document.UserName, document.PasswordHash, role);
        return result.IsSuccess
            ? result.Value
            : throw new InvalidDataException($"Invalid user document '{document.Id}': {result.Error.Code}");
    }

    private static UserDocument ToDocument(User user) => new()
    {
        Id = user.Id,
        UserName = user.UserName.Value,
        PasswordHash = user.PasswordHash.Value,
        Role = user.Role.ToString()
    };

    public sealed class UserDocument
    {
        public UserDocument() { }
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid Id { get; set; }
        [BsonElement("userName")] public string UserName { get; set; } = string.Empty;
        [BsonElement("passwordHash")] public string PasswordHash { get; set; } = string.Empty;
        [BsonElement("role")] public string Role { get; set; } = string.Empty;
    }
}
