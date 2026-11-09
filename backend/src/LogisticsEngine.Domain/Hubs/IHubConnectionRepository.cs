using LogisticsEngine.Domain.Hubs.Vo;

namespace LogisticsEngine.Domain.Hubs;

public interface IHubConnectionRepository
{
    IReadOnlyList<HubConnection> GetAll();
    void Add(HubConnection connection);
}
