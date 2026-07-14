using Dhexby.LogisticsEngine.Domain.Zones;

namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>Modello di persistenza della zona (solo primitivi).</summary>
internal sealed record ZoneDocument(
    Guid Id,
    string Code,
    string Name,
    Guid? ParentZoneId,
    IReadOnlyList<StreetDocument> Streets,
    DateTime CreatedOnUtc)
{
    public static ZoneDocument FromDomain(Zone zone) => new(
        zone.Id,
        zone.Code.Value,
        zone.Name,
        zone.ParentZoneId,
        zone.Streets.Select(StreetDocument.FromDomain).ToArray(),
        zone.CreatedOnUtc);

    public Zone ToDomain() => Zone.Rehydrate(
        Id,
        ZoneCode.Create(Code).Value,
        Name,
        ParentZoneId,
        Streets.Select(street => street.ToDomain()),
        CreatedOnUtc);
}

internal sealed record StreetDocument(string Name, string PostalCode)
{
    public static StreetDocument FromDomain(Street street) => new(street.Name, street.PostalCode);

    public Street ToDomain() => Street.Create(Name, PostalCode).Value;
}
