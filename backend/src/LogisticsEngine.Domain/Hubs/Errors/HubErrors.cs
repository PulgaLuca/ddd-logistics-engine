using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Domain.Hubs.Errors;

public static class HubErrors
{
    public static readonly Error InvalidId = Error.Validation("Hub.InvalidId", "The hub ID cannot be empty.");
    public static readonly Error InvalidZone = Error.Validation("Hub.InvalidZone", "The zone must contain 2 to 10 ASCII letters.");
    public static readonly Error InvalidConnection = Error.Validation("Hub.InvalidConnection", "A connection requires two different Hub IDs and a positive distance.");
    public static readonly Error PathNotFound = Error.NotFound("Hub.PathNotFound", "No route connects the selected Hubs.");
    public static readonly Error InvalidPathLimit = Error.Validation("Hub.InvalidPathLimit", "The maximum number of paths must be positive.");
    public static readonly Error DuplicateConnection = Error.Validation("Hub.DuplicateConnection", "A connection between these Hubs already exists.");
    public static Error NotFound(Guid id) => Error.NotFound("Hub.NotFound", $"Hub '{id}' was not found.");
    public static Error NotFoundByName(string name) => Error.NotFound("Hub.NotFound", $"Hub '{name}' was not found.");
    public static Error DuplicateName(string name) => Error.Validation("Hub.DuplicateName", $"An hub named '{name}' already exists.");
}
