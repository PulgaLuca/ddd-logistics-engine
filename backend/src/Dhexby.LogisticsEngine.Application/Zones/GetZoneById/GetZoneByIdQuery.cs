using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Zones.GetZoneById;

public sealed record GetZoneByIdQuery(Guid ZoneId) : IRequest<Result<ZoneResponse>>;
