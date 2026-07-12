using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Shipments;

/// <summary>
/// Catalogo centralizzato degli errori di dominio della spedizione (DRY:
/// ogni regola violata ha un solo codice, riusato da handler e test).
/// </summary>
public static class ShipmentErrors
{
    public static readonly Error TrackingNumberEmpty = Error.Failure(
        "Shipment.TrackingNumber.Empty",
        "The tracking number is required.");

    public static readonly Error TrackingNumberInvalidFormat = Error.Failure(
        "Shipment.TrackingNumber.InvalidFormat",
        "The tracking number must match the format TRK-XXXXXXXX (8 uppercase letters or digits).");

    public static readonly Error TrackingNumberAlreadyInUse = Error.Conflict(
        "Shipment.TrackingNumber.AlreadyInUse",
        "A shipment with the same tracking number already exists.");

    public static readonly Error WeightNotPositive = Error.Failure(
        "Shipment.Weight.NotPositive",
        "The weight must be greater than zero.");

    public static readonly Error WeightTooHeavy = Error.Failure(
        "Shipment.Weight.TooHeavy",
        $"The weight cannot exceed {Weight.MaxKilograms} kg.");

    public static readonly Error AddressIncomplete = Error.Failure(
        "Shipment.Address.Incomplete",
        "Street, city, postal code and country are all required.");

    public static readonly Error MoneyNegative = Error.Failure(
        "Shipment.Money.Negative",
        "A price cannot be negative.");

    public static readonly Error MoneyInvalidCurrency = Error.Failure(
        "Shipment.Money.InvalidCurrency",
        "The currency must be a 3-letter ISO 4217 code.");

    public static readonly Error SameOriginAndDestination = Error.Failure(
        "Shipment.SameOriginAndDestination",
        "Origin and destination addresses must be different.");

    public static readonly Error NotPending = Error.Conflict(
        "Shipment.NotPending",
        "Only a pending shipment can be dispatched.");

    public static readonly Error NotInTransit = Error.Conflict(
        "Shipment.NotInTransit",
        "Only a shipment in transit can be delivered.");

    public static readonly Error CannotCancel = Error.Conflict(
        "Shipment.CannotCancel",
        "Only a pending shipment can be cancelled.");

    public static Error NotFound(Guid shipmentId) => Error.NotFound(
        "Shipment.NotFound",
        $"The shipment with id '{shipmentId}' was not found.");
}
