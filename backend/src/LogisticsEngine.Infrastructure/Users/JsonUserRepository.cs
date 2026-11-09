using System.Text.Json;
using LogisticsEngine.Domain.Users;
using LogisticsEngine.Domain.Users.Entities;
using LogisticsEngine.Domain.Users.Enums;
using LogisticsEngine.Domain.Users.ValueObjects;

namespace LogisticsEngine.Infrastructure.Users;

/// <summary>Persiste gli utenti in users.json; il documento contiene solo l'hash, mai la password in chiaro.</summary>
public sealed class JsonUserRepository(string dataDirectory) : IUserRepository
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };
    private static readonly object _fileLock = new();
    private readonly string _directory = Path.GetFullPath(dataDirectory);
    private string FilePath => Path.Combine(_directory, "users.json");

    public User? GetById(Guid id)
    {
        lock (_fileLock)
        {
            return ReadAll().FirstOrDefault(user => user.Id == id);
        }
    }

    public User? GetByUserName(UserName userName)
    {
        ArgumentNullException.ThrowIfNull(userName);
        lock (_fileLock)
        {
            return ReadAll().FirstOrDefault(user =>
                string.Equals(user.UserName.Value, userName.Value, StringComparison.Ordinal));
        }
    }

    public void Add(User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        lock (_fileLock)
        {
            List<UserDocument> documents = ReadDocuments();
            if (documents.Any(document => document.Id == user.Id ||
                string.Equals(document.UserName, user.UserName.Value, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("User ID or user name already exists.");
            }

            documents.Add(ToDocument(user));
            WriteAll(documents);
        }
    }

    private List<User> ReadAll() => ReadDocuments().Select(ToEntity).ToList();

    private List<UserDocument> ReadDocuments()
    {
        if (!File.Exists(FilePath))
        {
            return [];
        }

        UserDocument[]? documents = JsonSerializer.Deserialize<UserDocument[]>(File.ReadAllText(FilePath), _jsonOptions);
        if (documents is null || documents.Any(document => document is null))
        {
            throw new InvalidDataException($"Invalid user document: {FilePath}");
        }

        return documents.ToList();
    }

    private static User ToEntity(UserDocument document)
    {
        if (!Enum.TryParse(document.Role, ignoreCase: false, out UserRole role))
        {
            throw new InvalidDataException($"Invalid user role for '{document.Id}'.");
        }

        var result = User.Create(document.Id, document.UserName, document.PasswordHash, role);
        return result.IsSuccess
            ? result.Value
            : throw new InvalidDataException($"Invalid user data for '{document.Id}': {result.Error.Code}");
    }

    private static UserDocument ToDocument(User user) =>
        new(user.Id, user.UserName.Value, user.PasswordHash.Value, user.Role.ToString());

    private void WriteAll(IReadOnlyCollection<UserDocument> documents)
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

    private sealed record UserDocument(Guid Id, string UserName, string PasswordHash, string Role);
}
