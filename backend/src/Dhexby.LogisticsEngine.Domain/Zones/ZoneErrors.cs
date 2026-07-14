using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Zones;

/// <summary>Catalogo centralizzato (DRY) degli errori di dominio della zona.</summary>
public static class ZoneErrors
{
    public static readonly Error CodeEmpty = Error.Failure(
        "Zone.Code.Empty",
        "The zone code is required.");

    public static readonly Error CodeInvalidFormat = Error.Failure(
        "Zone.Code.InvalidFormat",
        "The zone code must match the format ZON-XXXX (4 uppercase letters or digits).");

    public static readonly Error CodeAlreadyInUse = Error.Conflict(
        "Zone.Code.AlreadyInUse",
        "A zone with the same code already exists.");

    public static readonly Error NameRequired = Error.Failure(
        "Zone.Name.Required",
        "The zone name is required.");

    public static readonly Error NameTooLong = Error.Failure(
        "Zone.Name.TooLong",
        $"The zone name cannot exceed {Zone.MaxNameLength} characters.");

    public static readonly Error StreetIncomplete = Error.Failure(
        "Zone.Street.Incomplete",
        "The street name and postal code are both required.");

    public static readonly Error DuplicateStreet = Error.Conflict(
        "Zone.Street.Duplicate",
        "The street already belongs to this zone.");

    public static readonly Error ParentIsSelf = Error.Failure(
        "Zone.Parent.Self",
        "A zone cannot be its own parent.");

    public static Error NotFound(Guid zoneId) => Error.NotFound(
        "Zone.NotFound",
        $"The zone with id '{zoneId}' was not found.");

    public static Error ParentNotFound(Guid parentZoneId) => Error.NotFound(
        "Zone.Parent.NotFound",
        $"The parent zone with id '{parentZoneId}' was not found.");
}
