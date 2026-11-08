using LogisticsEngine.Domain.Shipments.Entities;

namespace LogisticsEngine.Domain.Shipments;

public interface IShipmentRepository
{
    void Add(Shipment shipment);
    Shipment? GetById(Guid id);
    IReadOnlyList<Shipment> GetAll();
    bool Update(Shipment shipment);
    bool Delete(Guid id);
}
