using LogisticsEngine.Domain.Hubs;

namespace LogisticsEngine.Application.Hubs;

public sealed record HubDto(Guid Id, string Name, string Zone)
{
    public static HubDto From(Hub hub) => new(hub.Id, hub.Name, hub.Zone.Code);
}
