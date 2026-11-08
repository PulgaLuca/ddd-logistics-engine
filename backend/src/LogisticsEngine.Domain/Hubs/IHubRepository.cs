using LogisticsEngine.Domain.Hubs.Entities;
using LogisticsEngine.Domain.Shared.Names;

namespace LogisticsEngine.Domain.Hubs;

public interface IHubRepository
{
    Hub? GetById(Guid id);
    Hub? GetByName(Name name);
    IReadOnlyList<Hub> GetAll();
    void Add(Hub hub);
    bool Update(Hub hub);
    bool Delete(Guid id);
}
