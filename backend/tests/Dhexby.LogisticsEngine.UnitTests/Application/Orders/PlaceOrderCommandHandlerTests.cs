using Dhexby.LogisticsEngine.Application.Orders.PlaceOrder;
using Dhexby.LogisticsEngine.Domain.Orders;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.UnitTests.Application.Orders;

public class PlaceOrderCommandHandlerTests
{
    private readonly InMemoryOrderRepository _repository = new();
    private readonly PlaceOrderCommandHandler _handler;

    public PlaceOrderCommandHandlerTests()
    {
        _handler = new PlaceOrderCommandHandler(_repository);
    }

    private static PlaceOrderCommand ValidCommand(string orderNumber = "ORD-ABC12345") =>
        new(orderNumber, "Mario Rossi", "Express");

    [Fact]
    public async Task Handle_ShouldPersistOrder_AndReturnId()
    {
        Result<Guid> result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Order? saved = await _repository.GetByIdAsync(result.Value);
        Assert.NotNull(saved);
        Assert.Equal(OrderPriority.Express, saved.Priority);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenPriorityUnknown()
    {
        Result<Guid> result = await _handler.Handle(
            ValidCommand() with { Priority = "SuperUrgent" },
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(OrderErrors.UnknownPriority, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenOrderNumberDuplicated()
    {
        await _handler.Handle(ValidCommand(), CancellationToken.None);

        Result<Guid> result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(OrderErrors.OrderNumberAlreadyInUse, result.Error);
    }
}
