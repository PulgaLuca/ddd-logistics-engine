using Dhexby.LogisticsEngine.Application.Abstractions.Persistence;

namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>
/// Unit of Work neutro per i provider Json e Mongo (standalone), che scrivono
/// in modo immediato e non offrono transazioni multi-documento.
/// <para>
/// Serve a mantenere identico il contratto della pipeline qualunque sia lo
/// store configurato: il <c>TransactionBehavior</c> resta invariato, cambia solo
/// la garanzia transazionale effettiva. È il prezzo esplicito — e documentato —
/// di scegliere un provider non transazionale.
/// </para>
/// </summary>
internal sealed class NoOpUnitOfWork : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(0);

    public Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<ITransaction>(new NoOpTransaction());

    private sealed class NoOpTransaction : ITransaction
    {
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task RollbackAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
