using Dhexby.LogisticsEngine.Domain.Zones;

namespace Dhexby.LogisticsEngine.Application.Zones;

/// <summary>DTO di lettura della zona (con le vie che contiene).</summary>
public sealed record ZoneResponse(
    Guid Id,
    string Code,
    string Name,
    Guid? ParentZoneId,
    IReadOnlyList<StreetResponse> Streets,
    DateTime CreatedOnUtc)
{
    public static ZoneResponse FromDomain(Zone zone) => new(
        zone.Id,
        zone.Code.Value,
        zone.Name,
        zone.ParentZoneId,
        zone.Streets.Select(street => new StreetResponse(street.Name, street.PostalCode)).ToArray(),
        zone.CreatedOnUtc);
}

public sealed record StreetResponse(string Name, string PostalCode);
