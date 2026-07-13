using Dhexby.LogisticsEngine.Domain.Zones;
using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Zones.GetZoneTree;

/// <summary>
/// Use case: costruisce l'albero delle zone a partire dalla radice, scendendo
/// ricorsivamente sulle sottozone. Un insieme di visitati evita cicli in caso
/// di dati incoerenti.
/// </summary>
public sealed class GetZoneTreeQueryHandler : IRequestHandler<GetZoneTreeQuery, Result<ZoneTreeResponse>>
{
    private readonly IZoneRepository _zoneRepository;

    public GetZoneTreeQueryHandler(IZoneRepository zoneRepository)
    {
        _zoneRepository = zoneRepository;
    }

    public async Task<Result<ZoneTreeResponse>> Handle(GetZoneTreeQuery request, CancellationToken cancellationToken)
    {
        Zone? root = await _zoneRepository.GetByIdAsync(request.RootZoneId, cancellationToken);
        if (root is null)
        {
            return Result.Failure<ZoneTreeResponse>(ZoneErrors.NotFound(request.RootZoneId));
        }

        return await BuildNodeAsync(root, [], cancellationToken);
    }

    private async Task<ZoneTreeResponse> BuildNodeAsync(
        Zone zone,
        HashSet<Guid> visited,
        CancellationToken cancellationToken)
    {
        visited.Add(zone.Id);

        IReadOnlyList<Zone> children = await _zoneRepository.GetChildrenAsync(zone.Id, cancellationToken);

        var subzones = new List<ZoneTreeResponse>(children.Count);
        foreach (Zone child in children)
        {
            if (visited.Contains(child.Id))
            {
                continue;
            }

            subzones.Add(await BuildNodeAsync(child, visited, cancellationToken));
        }

        return ZoneTreeResponse.FromNode(zone, subzones);
    }
}
