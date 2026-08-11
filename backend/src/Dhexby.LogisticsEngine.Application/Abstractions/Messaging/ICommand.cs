using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Abstractions.Messaging;

/// <summary>
/// Marker comune a tutti i comandi. Serve alla pipeline per distinguere le
/// scritture (da avvolgere in una transazione) dalle query di sola lettura,
/// senza ricorrere a convenzioni sul nome del tipo.
/// </summary>
public interface IBaseCommand;

/// <summary>Comando che non restituisce un valore (solo esito).</summary>
public interface ICommand : IRequest<Result>, IBaseCommand;

/// <summary>Comando che restituisce un valore in caso di successo.</summary>
/// <typeparam name="TResponse">Payload restituito dall'handler.</typeparam>
public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand;
