namespace Dhexby.LogisticsEngine.Application.Abstractions.Persistence;

/// <summary>
/// Unit of Work: definisce il confine transazionale di un use case. I repository
/// registrano le modifiche in memoria, ma è l'Unit of Work a decidere QUANDO
/// renderle definitive, così che più aggregati modificati dallo stesso comando
/// vengano salvati tutto-o-niente.
/// <para>
/// Il contratto vive in Application (il confine transazionale è una scelta
/// applicativa, non di dominio) e viene implementato in Infrastructure:
/// su PostgreSQL da una transazione reale, sui provider Json/Mongo da una
/// implementazione neutra (quegli store scrivono già in modo immediato).
/// </para>
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Rende persistenti tutte le modifiche tracciate dal contesto corrente.
    /// </summary>
    /// <returns>Numero di righe/documenti interessati.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Apre una nuova transazione. Se non viene mai chiamata <see cref="ITransaction.CommitAsync"/>,
    /// il dispose della transazione deve annullare le modifiche.
    /// </summary>
    Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
