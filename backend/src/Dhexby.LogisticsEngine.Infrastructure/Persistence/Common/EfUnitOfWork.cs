using Dhexby.LogisticsEngine.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>
/// Unit of Work su EF Core/PostgreSQL: il change tracker del
/// <see cref="LogisticsDbContext"/> accumula le modifiche fatte dai repository e
/// questa classe le rende definitive dentro una transazione ACID reale.
/// </summary>
internal sealed class EfUnitOfWork : IUnitOfWork
{
    private readonly LogisticsDbContext _context;

    public EfUnitOfWork(LogisticsDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);

    public async Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        // Con la retry strategy attiva EF può rieseguire il blocco: se una
        // transazione è già aperta (comando annidato) la si riusa invece di
        // aprirne una seconda, che PostgreSQL rifiuterebbe.
        IDbContextTransaction? current = _context.Database.CurrentTransaction;

        return current is not null
            ? new EfTransaction(current, ownsTransaction: false)
            : new EfTransaction(
                await _context.Database.BeginTransactionAsync(cancellationToken),
                ownsTransaction: true);
    }

    /// <summary>
    /// Adattatore fra la transazione di EF Core e il contratto applicativo
    /// <see cref="ITransaction"/>: il layer Application non deve conoscere EF.
    /// </summary>
    private sealed class EfTransaction : ITransaction
    {
        private readonly IDbContextTransaction _transaction;
        private readonly bool _ownsTransaction;

        public EfTransaction(IDbContextTransaction transaction, bool ownsTransaction)
        {
            _transaction = transaction;
            _ownsTransaction = ownsTransaction;
        }

        public Task CommitAsync(CancellationToken cancellationToken = default) =>
            _ownsTransaction ? _transaction.CommitAsync(cancellationToken) : Task.CompletedTask;

        public Task RollbackAsync(CancellationToken cancellationToken = default) =>
            _ownsTransaction ? _transaction.RollbackAsync(cancellationToken) : Task.CompletedTask;

        /// <summary>
        /// Dispose senza commit = rollback: è la rete di sicurezza che copre
        /// le eccezioni sollevate dagli handler.
        /// </summary>
        public ValueTask DisposeAsync() =>
            _ownsTransaction ? _transaction.DisposeAsync() : ValueTask.CompletedTask;
    }
}
