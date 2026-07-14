using Dhexby.LogisticsEngine.Domain.Zones;
using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Zones.CreateZone;

/// <summary>
/// Use case: crea una zona radice o una sottozona. Se è indicato un genitore,
/// ne verifica l'esistenza per mantenere consistente l'albero delle zone.
/// </summary>
public sealed class CreateZoneCommandHandler : IRequestHandler<CreateZoneCommand, Result<Guid>>
{
    private readonly IZoneRepository _zoneRepository;

    public CreateZoneCommandHandler(IZoneRepository zoneRepository)
    {
        _zoneRepository = zoneRepository;
    }

    public async Task<Result<Guid>> Handle(CreateZoneCommand request, CancellationToken cancellationToken)
    {
        Result<ZoneCode> codeResult = ZoneCode.Create(request.Code);
        if (codeResult.IsFailure)
        {
            return Result.Failure<Guid>(codeResult.Error);
        }

        if (request.ParentZoneId is { } parentId)
        {
            Zone? parent = await _zoneRepository.GetByIdAsync(parentId, cancellationToken);
            if (parent is null)
            {
                return Result.Failure<Guid>(ZoneErrors.ParentNotFound(parentId));
            }
        }

        bool codeInUse = await _zoneRepository.ExistsByCodeAsync(codeResult.Value, cancellationToken);
        if (codeInUse)
        {
            return Result.Failure<Guid>(ZoneErrors.CodeAlreadyInUse);
        }

        Result<Zone> zoneResult = Zone.Create(codeResult.Value, request.Name, request.ParentZoneId, DateTime.UtcNow);
        if (zoneResult.IsFailure)
        {
            return Result.Failure<Guid>(zoneResult.Error);
        }

        await _zoneRepository.AddAsync(zoneResult.Value, cancellationToken);

        return zoneResult.Value.Id;
    }
}
