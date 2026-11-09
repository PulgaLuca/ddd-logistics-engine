using LogisticsEngine.Domain.Users.Entities;

namespace LogisticsEngine.Application.Users;

/// <summary>Identità pubblica priva di password e hash.</summary>
public sealed record UserDto(Guid Id, string UserName, string Role)
{
    public static UserDto From(User user) => new(user.Id, user.UserName.Value, user.Role.ToString());
}
