using System.Reflection;
using static System.Net.Mime.MediaTypeNames;

namespace LogisticsEngine.ArchitectureTests;

/// <summary>Punti di ancoraggio per risolvere gli assembly dei layer.</summary>
internal static class ArchitectureAssemblies
{
    public static readonly Assembly Domain = typeof(Shipment).Assembly;
    public static readonly Assembly Application = typeof(Application.DependencyInjection).Assembly;
    public static readonly Assembly Infrastructure = typeof(Infrastructure.DependencyInjection).Assembly;
    public static readonly Assembly Api = typeof(ShipmentsController).Assembly;
}
