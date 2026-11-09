using LogisticsEngine.Domain.Hubs;
using LogisticsEngine.Domain.Shipments;
using LogisticsEngine.Domain.Users;
using LogisticsEngine.Application.Users;
using LogisticsEngine.Infrastructure.Users;
using LogisticsEngine.Infrastructure.Hubs;
using LogisticsEngine.Infrastructure.Shipments;
using Microsoft.Extensions.DependencyInjection;

namespace LogisticsEngine.Infrastructure;

// Extension method for DI container for earch layer
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string dataDirectory, string? tokenSigningKey = null)
    {
        services.AddSingleton<IHubRepository>(new JsonHubRepository(dataDirectory));
        services.AddSingleton<IHubConnectionRepository>(new JsonHubConnectionRepository(dataDirectory));
        services.AddSingleton<IShipmentRepository>(new JsonShipmentRepository(dataDirectory));
        services.AddSingleton<IUserRepository>(new JsonUserRepository(dataDirectory));
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddSingleton<IUserAuthenticationService, JsonUserAuthenticationService>();
        services.AddSingleton<IUserAuthorizationService, DomainUserAuthorizationService>();
        string signingKey = tokenSigningKey ?? Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));
        var accessTokenService = new HmacUserAccessTokenService(signingKey);
        services.AddSingleton<IUserAccessTokenService>(accessTokenService);
        services.AddSingleton<IUserAccessTokenValidator>(accessTokenService);
        return services;
    }
}
