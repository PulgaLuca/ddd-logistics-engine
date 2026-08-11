namespace Dhexby.LogisticsEngine.Application.Abstractions.Persistence;

/// <summary>
/// Transazione applicativa aperta da <see cref="IUnitOfWork.BeginTransactionAsync"/>.
/// Il dispose senza commit equivale a un rollback: è questa la garanzia che rende
/// sicuro il <c>await using</c> nella pipeline, anche quando l'handler lancia.
/// </summary>
public interface ITransaction : IAsyncDisposable
{
    /// <summary>Conferma definitivamente le modifiche.</summary>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>Annulla le modifiche effettuate all'interno della transazione.</summary>
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
