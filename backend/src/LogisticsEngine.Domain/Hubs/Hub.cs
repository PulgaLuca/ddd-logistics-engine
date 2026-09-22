using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Domain.Hubs;

public sealed class Hub : Entity
{
    private Hub(Guid id, string name, Zone zone) : base(id)
    {
        Name = name;
        Zone = zone;
    }

    public string Name { get; private set; }
    public Zone Zone { get; private set; }

    public static Result<Hub> Create(Guid id, string? name, string? zoneCode)
    {
        if (id == Guid.Empty)
        {
            return Result.Failure<Hub>(HubErrors.InvalidId);
        }

        string normalizedName = name?.Trim() ?? string.Empty;
        if (normalizedName.Length is < 1 or > 100)
        {
            return Result.Failure<Hub>(HubErrors.InvalidName);
        }

        Result<Zone> zone = Zone.Create(zoneCode);
        return zone.IsFailure
            ? Result.Failure<Hub>(zone.Error)
            : Result.Success(new Hub(id, normalizedName, zone.Value));
    }

    public Result Update(string? name, string? zoneCode)
    {
        Result<Hub> candidate = Create(Id, name, zoneCode);
        if (candidate.IsFailure)
        {
            return Result.Failure(candidate.Error);
        }

        // Applica entrambe le modifiche solo dopo aver validato tutti i dati.
        Name = candidate.Value.Name;
        Zone = candidate.Value.Zone;
        return Result.Success();
    }
}
