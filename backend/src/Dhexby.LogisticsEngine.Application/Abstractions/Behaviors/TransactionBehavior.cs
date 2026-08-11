using Dhexby.LogisticsEngine.Application.Abstractions.Messaging;
using Dhexby.LogisticsEngine.Application.Abstractions.Persistence;
using Dhexby.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dhexby.LogisticsEngine.Application.Abstractions.Behaviors;

/// <summary>
/// Pipeline behavior di MediatR che implementa il transactional pattern:
/// ogni <see cref="IBaseCommand"/> viene eseguito dentro una transazione
/// gestita dall'<see cref="IUnitOfWork"/>.
/// <list type="bullet">
///   <item>handler in successo → SaveChanges + Commit;</item>
///   <item>handler che ritorna un <see cref="Result"/> fallito → Rollback
///   (le regole di business violate non devono lasciare scritture parziali);</item>
///   <item>eccezione → il <c>await using</c> chiude la transazione senza commit,
///   quindi il database annulla tutto.</item>
/// </list>
/// Le query (che non implementano il marker) attraversano la pipeline senza
/// aprire alcuna transazione.
/// </summary>
/// <typeparam name="TRequest">Command o query in ingresso.</typeparam>
/// <typeparam name="TResponse">Deve essere un Result (o Result&lt;T&gt;).</typeparam>
public sealed class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
    where TResponse : Result
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

    public TransactionBehavior(
        IUnitOfWork unitOfWork,
        ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not IBaseCommand)
        {
            return await next(cancellationToken);
        }

        await using ITransaction transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        TResponse response = await next(cancellationToken);

        if (response.IsFailure)
        {
            TransactionLog.RollingBack(_logger, typeof(TRequest).Name, response.Error.Code);

            await transaction.RollbackAsync(cancellationToken);

            return response;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return response;
    }
}
