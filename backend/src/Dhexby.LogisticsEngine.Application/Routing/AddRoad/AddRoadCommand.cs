using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Routing.AddRoad;

public sealed record AddRoadCommand(
    Guid FromHubId,
    Guid ToHubId,
    double DistanceInKilometers,
    double TrafficFactor) : IRequest<Result<Guid>>;
