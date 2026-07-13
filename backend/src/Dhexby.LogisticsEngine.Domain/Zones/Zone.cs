using Dhexby.LogisticsEngine.Domain.Zones.Events;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Zones;

/// <summary>
/// Aggregate root della zona di consegna. Il riferimento opzionale
/// <see cref="ParentZoneId"/> modella l'albero (Zone -&gt; Subzona), mentre le
/// <see cref="Streets"/> sono le foglie. Ogni zona è aggregato a sé: la
/// gerarchia si ricostruisce per id, senza attraversare i confini transazionali.
/// </summary>
public sealed class Zone : Entity
{
    public const int MaxNameLength = 150;

    private readonly List<Street> _streets;

    private Zone(
        Guid id,
        ZoneCode code,
        string name,
        Guid? parentZoneId,
        List<Street> streets,
        DateTime createdOnUtc)
        : base(id)
    {
        Code = code;
        Name = name;
        ParentZoneId = parentZoneId;
        _streets = streets;
        CreatedOnUtc = createdOnUtc;
    }

    public ZoneCode Code { get; private set; }

    public string Name { get; private set; }

    public Guid? ParentZoneId { get; private set; }

    public IReadOnlyCollection<Street> Streets => _streets.AsReadOnly();

    public DateTime CreatedOnUtc { get; private set; }

    /// <summary>
    /// Factory: crea una zona radice (parentZoneId null) oppure una sottozona.
    /// </summary>
    public static Result<Zone> Create(ZoneCode code, string? name, Guid? parentZoneId, DateTime utcNow)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<Zone>(ZoneErrors.NameRequired);
        }

        string trimmedName = name.Trim();
        if (trimmedName.Length > MaxNameLength)
        {
            return Result.Failure<Zone>(ZoneErrors.NameTooLong);
        }

        var zone = new Zone(Guid.NewGuid(), code, trimmedName, parentZoneId, [], utcNow);
        zone.RaiseDomainEvent(new ZoneCreatedDomainEvent(zone.Id, parentZoneId));

        return zone;
    }

    /// <summary>Business rule: una via non può essere aggiunta due volte alla stessa zona.</summary>
    public Result AddStreet(Street street)
    {
        ArgumentNullException.ThrowIfNull(street);

        if (_streets.Contains(street))
        {
            return Result.Failure(ZoneErrors.DuplicateStreet);
        }

        _streets.Add(street);
        RaiseDomainEvent(new StreetAddedToZoneDomainEvent(Id, street.Name));

        return Result.Success();
    }

    /// <summary>Ricostruisce l'aggregato dallo store senza eventi né regole.</summary>
    public static Zone Rehydrate(
        Guid id,
        ZoneCode code,
        string name,
        Guid? parentZoneId,
        IEnumerable<Street> streets,
        DateTime createdOnUtc) =>
        new(id, code, name, parentZoneId, [.. streets], createdOnUtc);
}
