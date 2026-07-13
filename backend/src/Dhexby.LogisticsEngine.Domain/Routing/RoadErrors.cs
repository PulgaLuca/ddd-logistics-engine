using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Routing;

/// <summary>Catalogo centralizzato (DRY) degli errori di dominio della strada.</summary>
public static class RoadErrors
{
    public static readonly Error HubRequired = Error.Failure(
        "Road.Hub.Required",
        "Both the origin and destination hub ids are required.");

    public static readonly Error SameHubs = Error.Failure(
        "Road.SameHubs",
        "A road must connect two different hubs.");

    public static readonly Error DistanceNotPositive = Error.Failure(
        "Road.Distance.NotPositive",
        "The road distance must be greater than zero.");

    public static readonly Error TrafficFactorOutOfRange = Error.Failure(
        "Road.TrafficFactor.OutOfRange",
        $"The traffic factor must be between {Road.MinTrafficFactor} and {Road.MaxTrafficFactor}.");

    public static readonly Error AlreadyExists = Error.Conflict(
        "Road.AlreadyExists",
        "A road already connects these two hubs.");

    public static Error NotFound(Guid roadId) => Error.NotFound(
        "Road.NotFound",
        $"The road with id '{roadId}' was not found.");
}
