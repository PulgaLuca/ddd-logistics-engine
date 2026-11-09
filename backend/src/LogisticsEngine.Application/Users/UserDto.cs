using LogisticsEngine.Domain.Users.Entities;

namespace LogisticsEngine.Application.Users;

/// <summary>Identità pubblica priva di password e hash.</summary>
public sealed record UserDto(Guid Id, string UserName, string Role, string? DisplayName = null,
    string FirstName = "", string LastName = "", string ProfessionalRole = "")
{
    public static UserDto From(User user, string? displayName = null) => new(user.Id, user.UserName.Value,
        user.Role.ToString(), string.IsNullOrWhiteSpace(displayName) ? user.DisplayName : displayName,
        user.FirstName, user.LastName, user.ProfessionalRole);
}
