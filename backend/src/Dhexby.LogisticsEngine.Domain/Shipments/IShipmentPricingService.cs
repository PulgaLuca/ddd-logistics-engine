using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Shipments;

/// <summary>
/// Domain service: calcola il prezzo di una spedizione. È un servizio di dominio
/// perché la regola non appartiene naturalmente a un singolo Value Object o entità.
/// </summary>
public interface IShipmentPricingService
{
    Result<Money> CalculatePrice(Weight weight, bool isExpress);
}
