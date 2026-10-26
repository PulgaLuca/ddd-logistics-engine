using Dhexby.LogisticsEngine.Domain.Hubs;

namespace Dhexby.LogisticsEngine.Application.Hubs;

/// <summary>DTO di lettura dell'hub: solo primitivi.</summary>
public sealed record HubResponse(
    Guid Id,
    string Code,
    double Latitude,
    double Longitude,
    string Status,
    DateTime CreatedOnUtc)
{
    public static HubResponse FromDomain(Hub hub) => new(
        hub.Id,
        hub.Code.Value,
        hub.Coordinate.Latitude,
        hub.Coordinate.Longitude,
        hub.Status.ToString(),
        hub.CreatedOnUtc);
}
