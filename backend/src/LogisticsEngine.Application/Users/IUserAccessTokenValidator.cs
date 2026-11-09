namespace LogisticsEngine.Application.Users;

/// <summary>Porta per verificare firma, integrità e scadenza di un token bearer.</summary>
public interface IUserAccessTokenValidator
{
    UserTokenClaims? Validate(string token);
}
