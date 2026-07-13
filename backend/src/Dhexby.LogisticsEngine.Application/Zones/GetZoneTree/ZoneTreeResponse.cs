using Dhexby.LogisticsEngine.Domain.Zones;

namespace Dhexby.LogisticsEngine.Application.Zones.GetZoneTree;

/// <summary>
/// Nodo dell'albero delle zone di consegna (struttura ad albero): la zona, le
/// sue vie (foglie) e ricorsivamente le sottozone.
/// </summary>
public sealed record ZoneTreeResponse(
    Guid ZoneId,
    string Code,
    string Name,
    IReadOnlyList<StreetResponse> Streets,
    IReadOnlyList<ZoneTreeResponse> Subzones)
{
    public static ZoneTreeResponse FromNode(Zone zone, IReadOnlyList<ZoneTreeResponse> subzones) => new(
        zone.Id,
        zone.Code.Value,
        zone.Name,
        zone.Streets.Select(street => new StreetResponse(street.Name, street.PostalCode)).ToArray(),
        subzones);
}
