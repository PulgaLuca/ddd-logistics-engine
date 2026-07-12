using System.Reflection;
using Dhexby.LogisticsEngine.Api.Controllers;
using Dhexby.LogisticsEngine.Application;
using Dhexby.LogisticsEngine.Domain.Shipments;
using Dhexby.LogisticsEngine.Infrastructure.Persistence;

namespace Dhexby.LogisticsEngine.ArchitectureTests;

/// <summary>Punti di ancoraggio per risolvere gli assembly dei layer.</summary>
internal static class ArchitectureAssemblies
{
    public static readonly Assembly Domain = typeof(Shipment).Assembly;
    public static readonly Assembly Application = typeof(DependencyInjection).Assembly;
    public static readonly Assembly Infrastructure = typeof(JsonShipmentRepository).Assembly;
    public static readonly Assembly Api = typeof(ShipmentsController).Assembly;
}
