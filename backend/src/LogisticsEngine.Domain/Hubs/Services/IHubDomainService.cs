using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Domain.Hubs.Services;

public interface IHubDomainService
{
    Result EnsureNameIsUnique(string? name, Guid? excludingHubId = null);
}
