using LogisticsEngine.Application.Hubs;
using LogisticsEngine.Application.Hubs.DeleteHub;
using LogisticsEngine.Application.Hubs.GetAll;
using LogisticsEngine.Application.Hubs.GetById;
using LogisticsEngine.Application.Hubs.RegisterHub;
using LogisticsEngine.Application.Hubs.UpdateHub;
using LogisticsEngine.Application.Shipments.CreateShipment;
using LogisticsEngine.Application.Shipments.Delete;
using LogisticsEngine.Application.Shipments.GetAll;
using LogisticsEngine.Application.Shipments.GetById;
using LogisticsEngine.Application.Shipments.UpdateStatus;
using LogisticsEngine.Domain.Hubs.Services;
using LogisticsEngine.Domain.Hubs.Entities;
using LogisticsEngine.Domain.Shared.Sorting;
using LogisticsEngine.SharedKernel;
using Microsoft.Extensions.DependencyInjection;
using LogisticsEngine.Application.Hubs.Validation;
using FluentValidation;
using LogisticsEngine.Application.Shared;
using LogisticsEngine.Application.Hubs.GetBy;
using LogisticsEngine.Application.Shipments;

namespace LogisticsEngine.Application;

/// <summary>Registra i servizi applicativi e i casi d'uso nel container di dependency injection.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IHubDomainService, HubDomainService>();
        services.AddSingleton<ISorter<Hub>, Sorter<Hub>>();
        services.AddSingleton<IHubSortingDomainService, HubSortingDomainService>();
        services.AddSingleton<IHubGroupingDomainService, HubGroupingDomainService>();

        services.AddTransient<IValidator<RegisterHubCommand>, RegisterHubCommandValidator>();
        services.AddTransient<IValidator<UpdateHubCommand>, UpdateHubCommandValidator>();
        services.AddTransient<IValidator<DeleteHubCommand>, DeleteHubCommandValidator>();
        services.AddTransient<IValidator<GetByIdCommand>, GetByIdCommandValidator>();
        services.AddTransient<IValidator<GetByNameCommand>, GetByNameCommandValidator>();

        // I command handler sono registrati tramite il contratto sincrono della demo.
        services.AddTransient<ICommandHandler<GetAllCommand, IReadOnlyList<HubDto>>, GetAllCommandHandler>();
        services.AddTransient<ICommandHandler<RegisterHubCommand, Result<HubDto>>, RegisterHubCommandHandler>();
        services.AddTransient<ICommandHandler<DeleteHubCommand, Result>, DeleteHubCommandHandler>();
        services.AddTransient<ICommandHandler<UpdateHubCommand, Result>, UpdateHubCommandHandler>();
        services.AddTransient<ICommandHandler<GetByIdCommand, Result<HubDto>>, GetByIdCommandHandler>();
        services.AddTransient<ICommandHandler<GetByNameCommand, Result<HubDto>>, GetByNameCommandHandler>();
        services.AddTransient<ICommandHandler<CreateShipmentCommand, Result<ShipmentDto>>, CreateShipmentCommandHandler>();
        services.AddTransient<ICommandHandler<GetAllShipmentsQuery, IReadOnlyList<ShipmentDto>>, GetAllShipmentsQueryHandler>();
        services.AddTransient<ICommandHandler<GetShipmentByIdQuery, Result<ShipmentDto>>, GetShipmentByIdQueryHandler>();
        services.AddTransient<ICommandHandler<UpdateShipmentStatusCommand, Result>, UpdateShipmentStatusCommandHandler>();
        services.AddTransient<ICommandHandler<DeleteShipmentCommand, Result>, DeleteShipmentCommandHandler>();
        return services;
    }
}
