using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Hubs.RegisterHub;

public sealed record RegisterHubCommand(
    string Code,
    string Name,
    double Latitude,
    double Longitude) : IRequest<Result<Guid>>;
