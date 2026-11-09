using LogisticsEngine.Domain.Users.Enums;
using LogisticsEngine.Domain.Users.ValueObjects;
using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Domain.Users.Entities;

/// <summary>Utente con credenziali rappresentate da hash e permessi derivati dal ruolo.</summary>
public sealed class User : Entity
{
    private User(Guid id, UserName userName, PasswordHash passwordHash, UserRole role, string firstName, string lastName, string professionalRole) : base(id)
    {
        UserName = userName;
        PasswordHash = passwordHash;
        Role = role;
        FirstName = firstName;
        LastName = lastName;
        ProfessionalRole = professionalRole;
    }

    public UserName UserName { get; }
    public PasswordHash PasswordHash { get; }
    public UserRole Role { get; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string ProfessionalRole { get; private set; }
    public string DisplayName => string.Join(" ", new[] { FirstName, LastName }.Where(value => !string.IsNullOrWhiteSpace(value)));

    public static Result<User> Create(Guid id, string? userName, string? passwordHash, UserRole role,
        string? firstName = null, string? lastName = null, string? professionalRole = null)
    {
        if (id == Guid.Empty)
        {
            return Result.Failure<User>(UserErrors.InvalidId);
        }

        if (!Enum.IsDefined(role))
        {
            return Result.Failure<User>(UserErrors.InvalidRole);
        }

        Result<UserName> validUserName = UserName.Create(userName);
        if (validUserName.IsFailure)
        {
            return Result.Failure<User>(validUserName.Error);
        }

        Result<PasswordHash> validHash = PasswordHash.Create(passwordHash);
        if (validHash.IsFailure)
        {
            return Result.Failure<User>(validHash.Error);
        }
        string first = firstName?.Trim() ?? string.Empty;
        string last = lastName?.Trim() ?? string.Empty;
        string position = professionalRole?.Trim() ?? string.Empty;
        if (first.Length > 80 || last.Length > 80 || position.Length > 80)
        {
            return Result.Failure<User>(UserErrors.InvalidProfile);
        }
            
        return Result.Success(new User(id, validUserName.Value, validHash.Value, role, first, last, position));
    }

    public Result<bool> UpdateProfile(string? firstName, string? lastName, string? professionalRole)
    {
        string first = firstName?.Trim() ?? string.Empty;
        string last = lastName?.Trim() ?? string.Empty;
        string position = professionalRole?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(first) || string.IsNullOrWhiteSpace(last) || first.Length > 80 || last.Length > 80 || position.Length > 80)
        {
            return Result.Failure<bool>(UserErrors.InvalidProfile);
        }
            
        FirstName = first;
        LastName = last;
        ProfessionalRole = position;
        return Result.Success(true);
    }

    public bool HasPermission(UserPermission permission) => Role switch
    {
        UserRole.Administrator => Enum.IsDefined(permission),
        UserRole.Operator => permission is UserPermission.ReadLogistics or UserPermission.ManageHubs or UserPermission.ManageShipments,
        UserRole.Viewer => permission is UserPermission.ReadLogistics,
        _ => false
    };
}
