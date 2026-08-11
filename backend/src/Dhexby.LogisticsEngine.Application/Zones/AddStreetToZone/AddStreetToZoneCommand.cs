using Dhexby.LogisticsEngine.Application.Abstractions.Messaging;

namespace Dhexby.LogisticsEngine.Application.Zones.AddStreetToZone;

public sealed record AddStreetToZoneCommand(
    Guid ZoneId,
    string StreetName,
    string PostalCode) : ICommand;
