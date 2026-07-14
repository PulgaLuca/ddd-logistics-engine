using Dhexby.LogisticsEngine.Domain.Zones;
using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Zones.AddStreetToZone;

/// <summary>Use case: aggiunge una via a una zona esistente.</summary>
public sealed class AddStreetToZoneCommandHandler : IRequestHandler<AddStreetToZoneCommand, Result>
{
    private readonly IZoneRepository _zoneRepository;

    public AddStreetToZoneCommandHandler(IZoneRepository zoneRepository)
    {
        _zoneRepository = zoneRepository;
    }

    public async Task<Result> Handle(AddStreetToZoneCommand request, CancellationToken cancellationToken)
    {
        Zone? zone = await _zoneRepository.GetByIdAsync(request.ZoneId, cancellationToken);
        if (zone is null)
        {
            return Result.Failure(ZoneErrors.NotFound(request.ZoneId));
        }

        Result<Street> streetResult = Street.Create(request.StreetName, request.PostalCode);
        if (streetResult.IsFailure)
        {
            return streetResult;
        }

        Result addResult = zone.AddStreet(streetResult.Value);
        if (addResult.IsFailure)
        {
            return addResult;
        }

        await _zoneRepository.UpdateAsync(zone, cancellationToken);

        return Result.Success();
    }
}
