using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Hubs;

/// <summary>Catalogo centralizzato (DRY) degli errori di dominio dell'hub.</summary>
public static class HubErrors
{
    public static readonly Error CodeEmpty = Error.Failure(
        "Hub.Code.Empty",
        "The hub code is required.");

    public static readonly Error CodeInvalidFormat = Error.Failure(
        "Hub.Code.InvalidFormat",
        "The hub code must match the format HUB-XXXX (4 uppercase letters or digits).");

    public static readonly Error CodeAlreadyInUse = Error.Conflict(
        "Hub.Code.AlreadyInUse",
        "A hub with the same code already exists.");

    public static readonly Error NameRequired = Error.Failure(
        "Hub.Name.Required",
        "The hub name is required.");

    public static readonly Error NameTooLong = Error.Failure(
        "Hub.Name.TooLong",
        $"The hub name cannot exceed {Hub.MaxNameLength} characters.");

    public static readonly Error LatitudeOutOfRange = Error.Failure(
        "Hub.Latitude.OutOfRange",
        "The latitude must be between -90 and 90 degrees.");

    public static readonly Error LongitudeOutOfRange = Error.Failure(
        "Hub.Longitude.OutOfRange",
        "The longitude must be between -180 and 180 degrees.");

    public static readonly Error AlreadyInactive = Error.Conflict(
        "Hub.AlreadyInactive",
        "The hub is already inactive.");

    public static readonly Error AlreadyActive = Error.Conflict(
        "Hub.AlreadyActive",
        "The hub is already active.");

    public static Error NotFound(Guid hubId) => Error.NotFound(
        "Hub.NotFound",
        $"The hub with id '{hubId}' was not found.");
}
