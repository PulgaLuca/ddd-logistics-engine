using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Orders.Dispatching.UndoDispatch;

/// <summary>Comando: annulla l'ultima assegnazione ordine -&gt; hub.</summary>
public sealed record UndoDispatchCommand : IRequest<Result<DispatchHistoryResponse>>;
