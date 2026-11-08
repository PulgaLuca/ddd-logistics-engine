using LogisticsEngine.Domain.Hubs.Errors;
using LogisticsEngine.Domain.Shared.Names;
using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Domain.Hubs.Services;

/// <summary>Controlla l'unicità del nome rispetto agli Hub persistiti.</summary>
public sealed class HubDomainService(IHubRepository repository) : IHubDomainService
{
    public Result EnsureNameIsUnique(string? name, Guid? excludingHubId = null)
    {
        Result<Name> candidate = Name.Create(name);
        if (candidate.IsFailure)
        {
            return Result.Failure(candidate.Error);
        }

        bool duplicateExists = repository.GetAll().Any(hub =>
            hub.Id != excludingHubId && string.Equals(hub.Name.Value, candidate.Value.Value, StringComparison.OrdinalIgnoreCase));
        return duplicateExists
            ? Result.Failure(NameErrors.DuplicateName("hub", candidate.Value.Value))
            : Result.Success();
    }
}
