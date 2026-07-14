using Dhexby.LogisticsEngine.Domain.Hubs;

namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>Modello di persistenza dell'hub (solo primitivi).</summary>
internal sealed record HubDocument(
    Guid Id,
    string Code,
    string Name,
    double Latitude,
    double Longitude,
    HubStatus Status,
    DateTime CreatedOnUtc)
{
    public static HubDocument FromDomain(Hub hub) => new(
        hub.Id,
        hub.Code.Value,
        hub.Name,
        hub.Coordinate.Latitude,
        hub.Coordinate.Longitude,
        hub.Status,
        hub.CreatedOnUtc);

    public Hub ToDomain() => Hub.Rehydrate(
        Id,
        HubCode.Create(Code).Value,
        Name,
        GeoCoordinate.Create(Latitude, Longitude).Value,
        Status,
        CreatedOnUtc);
}
