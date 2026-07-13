using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Orders.Dispatching.RedoDispatch;

/// <summary>Comando: ri-applica l'ultima assegnazione annullata.</summary>
public sealed record RedoDispatchCommand : IRequest<Result<DispatchHistoryResponse>>;
