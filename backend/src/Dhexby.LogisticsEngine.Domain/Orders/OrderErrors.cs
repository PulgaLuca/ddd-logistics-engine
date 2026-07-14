using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Orders;

/// <summary>Catalogo centralizzato (DRY) degli errori di dominio dell'ordine.</summary>
public static class OrderErrors
{
    public static readonly Error OrderNumberEmpty = Error.Failure(
        "Order.OrderNumber.Empty",
        "The order number is required.");

    public static readonly Error OrderNumberInvalidFormat = Error.Failure(
        "Order.OrderNumber.InvalidFormat",
        "The order number must match the format ORD-XXXXXXXX (8 uppercase letters or digits).");

    public static readonly Error OrderNumberAlreadyInUse = Error.Conflict(
        "Order.OrderNumber.AlreadyInUse",
        "An order with the same order number already exists.");

    public static readonly Error CustomerNameEmpty = Error.Failure(
        "Order.CustomerName.Empty",
        "The customer name is required.");

    public static readonly Error CustomerNameTooLong = Error.Failure(
        "Order.CustomerName.TooLong",
        $"The customer name cannot exceed {CustomerName.MaxLength} characters.");

    public static readonly Error UnknownPriority = Error.Failure(
        "Order.Priority.Unknown",
        "The order priority is not a valid value.");

    public static readonly Error HubIdRequired = Error.Failure(
        "Order.HubId.Required",
        "The hub id is required to assign the order.");

    public static readonly Error NotPending = Error.Conflict(
        "Order.NotPending",
        "Only a pending order can be assigned to a hub.");

    public static readonly Error NotAssigned = Error.Conflict(
        "Order.NotAssigned",
        "Only an assigned order can be unassigned.");

    public static readonly Error CannotComplete = Error.Conflict(
        "Order.CannotComplete",
        "Only an assigned order can be completed.");

    public static readonly Error CannotCancel = Error.Conflict(
        "Order.CannotCancel",
        "Only a pending or assigned order can be cancelled.");

    public static Error NotFound(Guid orderId) => Error.NotFound(
        "Order.NotFound",
        $"The order with id '{orderId}' was not found.");
}
