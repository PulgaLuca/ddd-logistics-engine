using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Domain.Hubs.Errors;

public static class NameErrors
{
    public static readonly Error InvalidName = Error.Validation("Hub.InvalidName", "The name must contain 1 to 120 characters.");
    public static Error DuplicateName(string entity, string name) => Error.Validation("DuplicateName", $"An '{entity}' named '{name}' already exists.");
}
