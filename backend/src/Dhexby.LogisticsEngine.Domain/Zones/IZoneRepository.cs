namespace Dhexby.LogisticsEngine.Domain.Zones;

/// <summary>
/// Porta di persistenza dell'aggregato Zone. <see cref="GetChildrenAsync"/>
/// restituisce le sottozone dirette di un nodo: è il mattone su cui costruire
/// in futuro l'albero delle zone di consegna (Zone -&gt; Subzona -&gt; Streets).
/// </summary>
public interface IZoneRepository
{
    Task<Zone?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(ZoneCode code, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Zone>> GetChildrenAsync(Guid parentZoneId, CancellationToken cancellationToken = default);

    Task AddAsync(Zone zone, CancellationToken cancellationToken = default);

    Task UpdateAsync(Zone zone, CancellationToken cancellationToken = default);
}
