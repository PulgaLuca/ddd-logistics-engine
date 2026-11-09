using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Domain.Hubs.Errors;

public static class HubErrors
{
    public static readonly Error InvalidId = Error.Validation("Hub.InvalidId", "The hub ID cannot be empty.");
    public static readonly Error InvalidZone = Error.Validation("Hub.InvalidZone", "The zone must contain 2 to 10 ASCII letters.");
    public static Error NotFound(Guid id) => Error.NotFound("Hub.NotFound", $"Hub '{id}' was not found.");
    public static Error NotFoundByName(string name) => Error.NotFound("Hub.NotFound", $"Hub '{name}' was not found.");
    public static Error DuplicateName(string name) => Error.Validation("Hub.DuplicateName", $"An hub named '{name}' already exists.");
}
