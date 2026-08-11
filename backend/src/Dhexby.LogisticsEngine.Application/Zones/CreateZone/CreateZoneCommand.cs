using Dhexby.LogisticsEngine.Application.Abstractions.Messaging;

namespace Dhexby.LogisticsEngine.Application.Zones.CreateZone;

public sealed record CreateZoneCommand(
    string Code,
    string Name,
    Guid? ParentZoneId) : ICommand<Guid>;
