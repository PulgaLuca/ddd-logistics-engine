using Dhexby.LogisticsEngine.Domain.Zones;
using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Zones.GetZoneById;

/// <summary>Use case di lettura: proiezione DTO della zona.</summary>
public sealed class GetZoneByIdQueryHandler : IRequestHandler<GetZoneByIdQuery, Result<ZoneResponse>>
{
    private readonly IZoneRepository _zoneRepository;

    public GetZoneByIdQueryHandler(IZoneRepository zoneRepository)
    {
        _zoneRepository = zoneRepository;
    }

    public async Task<Result<ZoneResponse>> Handle(GetZoneByIdQuery request, CancellationToken cancellationToken)
    {
        Zone? zone = await _zoneRepository.GetByIdAsync(request.ZoneId, cancellationToken);

        if (zone is null)
        {
            return Result.Failure<ZoneResponse>(ZoneErrors.NotFound(request.ZoneId));
        }

        return ZoneResponse.FromDomain(zone);
    }
}
