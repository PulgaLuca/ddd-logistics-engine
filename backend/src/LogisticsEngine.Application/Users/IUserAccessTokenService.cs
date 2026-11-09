namespace LogisticsEngine.Application.Users;

/// <summary>Porta per emettere token dopo l'autenticazione.</summary>
public interface IUserAccessTokenService
{
    UserAccessToken Create(UserDto user);
}
