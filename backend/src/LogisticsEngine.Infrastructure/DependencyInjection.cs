using LogisticsEngine.Domain.Hubs;
using Microsoft.Extensions.DependencyInjection;

namespace LogisticsEngine.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string dataDirectory) =>
        services.AddSingleton<IHubRepository>(new JsonHubRepository(dataDirectory));
}
