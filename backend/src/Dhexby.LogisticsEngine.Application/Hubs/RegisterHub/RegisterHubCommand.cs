using Dhexby.LogisticsEngine.Application.Abstractions.Messaging;

namespace Dhexby.LogisticsEngine.Application.Hubs.RegisterHub;

public sealed record RegisterHubCommand(
    string Code,
    string Name,
    double Latitude,
    double Longitude) : ICommand<Guid>;
