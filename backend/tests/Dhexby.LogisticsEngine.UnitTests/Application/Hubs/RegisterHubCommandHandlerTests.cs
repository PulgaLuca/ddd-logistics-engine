using Dhexby.LogisticsEngine.Application.Hubs.RegisterHub;
using Dhexby.LogisticsEngine.Domain.Hubs;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.UnitTests.Application.Hubs;

public class RegisterHubCommandHandlerTests
{
    private readonly InMemoryHubRepository _repository = new();
    private readonly RegisterHubCommandHandler _handler;

    public RegisterHubCommandHandlerTests()
    {
        _handler = new RegisterHubCommandHandler(_repository);
    }

    private static RegisterHubCommand ValidCommand(string code = "HUB-0001") =>
        new(code, "Milano Central", 45.4642, 9.1900);

    [Fact]
    public async Task Handle_ShouldPersistHub_AndReturnId()
    {
        Result<Guid> result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Hub? saved = await _repository.GetByIdAsync(result.Value);
        Assert.NotNull(saved);
        Assert.Equal("HUB-0001", saved.Code.Value);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCodeDuplicated()
    {
        await _handler.Handle(ValidCommand(), CancellationToken.None);

        Result<Guid> result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(HubErrors.CodeAlreadyInUse, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCoordinateInvalid()
    {
        Result<Guid> result = await _handler.Handle(
            ValidCommand() with { Latitude = 200 },
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(HubErrors.LatitudeOutOfRange, result.Error);
    }
}
