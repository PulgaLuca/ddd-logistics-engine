using Dhexby.LogisticsEngine.Domain.Orders;
using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Orders.PlaceOrder;

/// <summary>Use case: registra un nuovo ordine (stato iniziale Pending).</summary>
public sealed class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, Result<Guid>>
{
    private readonly IOrderRepository _orderRepository;

    public PlaceOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<Guid>> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        Result<OrderNumber> orderNumberResult = OrderNumber.Create(request.OrderNumber);
        if (orderNumberResult.IsFailure)
        {
            return Result.Failure<Guid>(orderNumberResult.Error);
        }

        Result<CustomerName> customerNameResult = CustomerName.Create(request.CustomerName);
        if (customerNameResult.IsFailure)
        {
            return Result.Failure<Guid>(customerNameResult.Error);
        }

        if (!Enum.TryParse(request.Priority, ignoreCase: true, out OrderPriority priority) ||
            !Enum.IsDefined(priority))
        {
            return Result.Failure<Guid>(OrderErrors.UnknownPriority);
        }

        bool orderNumberInUse = await _orderRepository.ExistsByOrderNumberAsync(
            orderNumberResult.Value,
            cancellationToken);

        if (orderNumberInUse)
        {
            return Result.Failure<Guid>(OrderErrors.OrderNumberAlreadyInUse);
        }

        Result<Order> orderResult = Order.Create(
            orderNumberResult.Value,
            customerNameResult.Value,
            priority,
            DateTime.UtcNow);

        if (orderResult.IsFailure)
        {
            return Result.Failure<Guid>(orderResult.Error);
        }

        await _orderRepository.AddAsync(orderResult.Value, cancellationToken);

        return orderResult.Value.Id;
    }
}
