using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Orders.Dispatching;

/// <summary>Errori di dominio relativi alla cronologia di dispatch (undo/redo).</summary>
public static class DispatchErrors
{
    public static readonly Error NothingToUndo = Error.Conflict(
        "Dispatch.NothingToUndo",
        "There is no dispatch assignment to undo.");

    public static readonly Error NothingToRedo = Error.Conflict(
        "Dispatch.NothingToRedo",
        "There is no dispatch assignment to redo.");
}
