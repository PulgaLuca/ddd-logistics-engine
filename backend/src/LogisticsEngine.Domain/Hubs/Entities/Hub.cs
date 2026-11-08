using LogisticsEngine.Domain.Hubs.Errors;
using LogisticsEngine.Domain.Hubs.Vo;
using LogisticsEngine.Domain.Shared.Names;
using LogisticsEngine.SharedKernel;
namespace LogisticsEngine.Domain.Hubs.Entities;

public sealed class Hub : Entity
{
    private Hub(Guid id, Name name, Zone zone) : base(id)
    {
        Name = name;
        Zone = zone;
    }

    public Name Name { get; private set; }
    public Zone Zone { get; private set; }

    public static Result<Hub> Create(Guid id, string? name, string? zoneCode)
    {
        if (id == Guid.Empty)
        {
            return Result.Failure<Hub>(HubErrors.InvalidId);
        }

        Result<Name> validName = Name.Create(name);
        if (validName.IsFailure)
        {
            return Result.Failure<Hub>(validName.Error);
        }

        Result<Zone> zone = Zone.Create(zoneCode);
        return zone.IsFailure ? Result.Failure<Hub>(zone.Error) : Result.Success(new Hub(id, validName.Value, zone.Value));
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
