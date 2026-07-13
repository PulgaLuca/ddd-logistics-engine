using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Zones.CreateZone;

public sealed record CreateZoneCommand(
    string Code,
    string Name,
    Guid? ParentZoneId) : IRequest<Result<Guid>>;
