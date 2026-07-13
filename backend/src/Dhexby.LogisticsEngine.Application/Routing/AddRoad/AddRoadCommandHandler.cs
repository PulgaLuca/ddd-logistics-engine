using Dhexby.LogisticsEngine.Domain.Hubs;
using Dhexby.LogisticsEngine.Domain.Routing;
using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Routing.AddRoad;

/// <summary>
/// Use case: aggiunge una strada (arco) tra due hub esistenti. Verifica la
/// presenza dei due hub e l'assenza di una strada già esistente tra loro.
/// </summary>
public sealed class AddRoadCommandHandler : IRequestHandler<AddRoadCommand, Result<Guid>>
{
    private readonly IRoadRepository _roadRepository;
    private readonly IHubRepository _hubRepository;

    public AddRoadCommandHandler(IRoadRepository roadRepository, IHubRepository hubRepository)
    {
        _roadRepository = roadRepository;
        _hubRepository = hubRepository;
    }

    public async Task<Result<Guid>> Handle(AddRoadCommand request, CancellationToken cancellationToken)
    {
        Hub? fromHub = await _hubRepository.GetByIdAsync(request.FromHubId, cancellationToken);
        if (fromHub is null)
        {
            return Result.Failure<Guid>(HubErrors.NotFound(request.FromHubId));
        }

        Hub? toHub = await _hubRepository.GetByIdAsync(request.ToHubId, cancellationToken);
        if (toHub is null)
        {
            return Result.Failure<Guid>(HubErrors.NotFound(request.ToHubId));
        }

        bool alreadyConnected = await _roadRepository.ExistsBetweenAsync(
            request.FromHubId,
            request.ToHubId,
            cancellationToken);

        if (alreadyConnected)
        {
            return Result.Failure<Guid>(RoadErrors.AlreadyExists);
        }

        Result<Road> roadResult = Road.Create(
            request.FromHubId,
            request.ToHubId,
            request.DistanceInKilometers,
            request.TrafficFactor,
            DateTime.UtcNow);

        if (roadResult.IsFailure)
        {
            return Result.Failure<Guid>(roadResult.Error);
        }

        await _roadRepository.AddAsync(roadResult.Value, cancellationToken);

        return roadResult.Value.Id;
    }
}
