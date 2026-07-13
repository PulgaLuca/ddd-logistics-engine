using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Zones.AddStreetToZone;

public sealed record AddStreetToZoneCommand(
    Guid ZoneId,
    string StreetName,
    string PostalCode) : IRequest<Result>;
