using FluentValidation;
using LogisticsEngine.Application.Hubs.DeleteHub;
using LogisticsEngine.Application.Hubs.GetBy;
using LogisticsEngine.Application.Hubs.GetById;
using LogisticsEngine.Application.Hubs.RegisterHub;
using LogisticsEngine.Application.Hubs.UpdateHub;
using LogisticsEngine.Domain.Hubs.Entities;
using LogisticsEngine.Domain.Hubs.Services;
using LogisticsEngine.Domain.Shared.Sorting;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace LogisticsEngine.Application;

/// <summary>Registra i request handler MediatR, i validator e i servizi usati dall'Application.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddTransient<IValidator<RegisterHubCommand>, RegisterHubCommandValidator>();
        services.AddTransient<IValidator<UpdateHubCommand>, UpdateHubCommandValidator>();
        services.AddTransient<IValidator<DeleteHubCommand>, DeleteHubCommandValidator>();
        services.AddTransient<IValidator<GetHubByIdQuery>, GetByIdCommandValidator>();
        services.AddTransient<IValidator<GetHubByNameQuery>, GetByNameCommandValidator>();

        services.AddSingleton<IHubDomainService, HubDomainService>();
        services.AddSingleton<ISorter<Hub>, Sorter<Hub>>();
        services.AddSingleton<IHubSortingDomainService, HubSortingDomainService>();
        services.AddSingleton<IHubGroupingDomainService, HubGroupingDomainService>();
        services.AddSingleton<IHubQueueDomainService, HubQueueDomainService>();
        services.AddSingleton<IHubStackDomainService, HubStackDomainService>();
        services.AddSingleton<IHubHashMapDomainService, HubHashMapDomainService>();
        services.AddSingleton<IHubSetDomainService, HubSetDomainService>();
        services.AddSingleton<IHubTreeDomainService, HubTreeDomainService>();
        services.AddSingleton<IHubTrieDomainService, HubTrieDomainService>();
        services.AddSingleton<IHubGraphDomainService, HubGraphDomainService>();
        services.AddSingleton<IHubBacktrackingDomainService, HubBacktrackingDomainService>();

        return services;
    }
}
