using Dhexby.LogisticsEngine.Domain.Hubs.Events;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Hubs;

/// <summary>
/// Aggregate root dell'hub: rappresenta un nodo della rete logistica.
/// La coordinata geografica lo rende pronto a diventare vertice di un grafo
/// per gli algoritmi di routing (Dijkstra / A* / BFS / DFS).
/// </summary>
public sealed class Hub : Entity
{
    private Hub(
        Guid id,
        HubCode code,
        GeoCoordinate coordinate,
        HubStatus status,
        DateTime createdOnUtc)
        : base(id)
    {
        Code = code;
        Coordinate = coordinate;
        Status = status;
        CreatedOnUtc = createdOnUtc;
    }

    public HubCode Code { get; private set; }

    public GeoCoordinate Coordinate { get; private set; }

    public HubStatus Status { get; private set; }

    public DateTime CreatedOnUtc { get; private set; }

    public static Result<Hub> Create(HubCode code, GeoCoordinate coordinate, DateTime utcNow)
    {   
        var hub = new Hub(Guid.NewGuid(), code, coordinate, HubStatus.Active, utcNow);
        hub.RaiseDomainEvent(new HubRegisteredDomainEvent(hub.Id));

        return hub;
    }

    /// <summary>Si disattiva solo un hub attivo.</summary>
    public Result Deactivate()
    {
        if (Status == HubStatus.Inactive)
            return Result.Failure(HubErrors.AlreadyInactive);

        Status = HubStatus.Inactive;
        RaiseDomainEvent(new HubDeactivatedDomainEvent(Id));

        return Result.Success();
    }

    /// <summary>Business rule: si riattiva solo un hub inattivo.</summary>
    public Result Activate()
    {
        if (Status == HubStatus.Active)
            return Result.Failure(HubErrors.AlreadyActive);

        Status = HubStatus.Active;

        return Result.Success();
    }

    /// <summary>Ricostruisce l'aggregato dallo store senza eventi né regole.</summary>
    public static Hub Rehydrate(
        Guid id,
        HubCode code,
        GeoCoordinate coordinate,
        HubStatus status,
        DateTime createdOnUtc) =>
        new(id, code, coordinate, status, createdOnUtc);
}
