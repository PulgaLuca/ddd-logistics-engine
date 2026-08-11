using Dhexby.LogisticsEngine.Application.Abstractions.Messaging;

namespace Dhexby.LogisticsEngine.Application.Routing.AddRoad;

public sealed record AddRoadCommand(
    Guid FromHubId,
    Guid ToHubId,
    double DistanceInKilometers,
    double TrafficFactor) : ICommand<Guid>;
