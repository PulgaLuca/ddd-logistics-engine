using LogisticsEngine.Domain.Users.Enums;
using LogisticsEngine.Domain.Users.ValueObjects;
using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Domain.Users.Entities;

/// <summary>Utente con credenziali rappresentate da hash e permessi derivati dal ruolo.</summary>
public sealed class User : Entity
{
    private User(Guid id, UserName userName, PasswordHash passwordHash, UserRole role) : base(id)
    {
        UserName = userName;
        PasswordHash = passwordHash;
        Role = role;
    }

    public UserName UserName { get; }
    public PasswordHash PasswordHash { get; }
    public UserRole Role { get; }

    public static Result<User> Create(Guid id, string? userName, string? passwordHash, UserRole role)
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
        return validHash.IsFailure
            ? Result.Failure<User>(validHash.Error)
            : Result.Success(new User(id, validUserName.Value, validHash.Value, role));
    }

    public bool HasPermission(UserPermission permission) => Role switch
    {
        UserRole.Administrator => Enum.IsDefined(permission),
        UserRole.Operator => permission is UserPermission.ReadLogistics or UserPermission.ManageHubs or UserPermission.ManageShipments,
        UserRole.Viewer => permission is UserPermission.ReadLogistics,
        _ => false
    };
}
