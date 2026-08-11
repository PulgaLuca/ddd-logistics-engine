using Dhexby.LogisticsEngine.Application.Abstractions.Messaging;

namespace Dhexby.LogisticsEngine.Application.Orders.Dispatching.RedoDispatch;

/// <summary>Comando: ri-applica l'ultima assegnazione annullata.</summary>
public sealed record RedoDispatchCommand : ICommand<DispatchHistoryResponse>;
