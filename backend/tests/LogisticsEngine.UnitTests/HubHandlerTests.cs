using LogisticsEngine.Application.Hubs;
using LogisticsEngine.Domain.Hubs;
using NSubstitute;

namespace LogisticsEngine.UnitTests;

public sealed class HubHandlerTests
{
    [Fact]
    public async Task Invalid_create_does_not_write_to_repository()
    {
        IHubRepository repository = Substitute.For<IHubRepository>();
        var handler = new CreateHubHandler(repository);
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        var result = await handler.Handle(new CreateHub("", "NORD"), cancellationToken);

        result.Error.ShouldBe(HubErrors.InvalidName);
        await repository.DidNotReceive().AddAsync(Arg.Any<Hub>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_persists_normalized_hub_and_returns_its_identity()
    {
        IHubRepository repository = Substitute.For<IHubRepository>();
        var handler = new CreateHubHandler(repository);
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        var result = await handler.Handle(new CreateHub(" Milano ", " nord "), cancellationToken);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Id.ShouldNotBe(Guid.Empty);
        result.Value.Name.ShouldBe("Milano");
        result.Value.Zone.ShouldBe("NORD");
        await repository.Received(1).AddAsync(
            Arg.Is<Hub>(hub => hub.Id == result.Value.Id && hub.Name == "Milano" && hub.Zone.Code == "NORD"),
            cancellationToken);
    }

    [Fact]
    public async Task Get_missing_hub_returns_not_found()
    {
        IHubRepository repository = Substitute.For<IHubRepository>();
        Guid id = Guid.NewGuid();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        repository.GetByIdAsync(id, cancellationToken).Returns((Hub?)null);

        var result = await new GetHubByIdHandler(repository).Handle(new GetHubById(id), cancellationToken);

        result.Error.ShouldBe(HubErrors.NotFound(id));
    }

    [Fact]
    public async Task Invalid_update_preserves_hub_and_does_not_persist()
    {
        IHubRepository repository = Substitute.For<IHubRepository>();
        Hub hub = Hub.Create(Guid.NewGuid(), "Milano", "NORD").Value;
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        repository.GetByIdAsync(hub.Id, cancellationToken).Returns(hub);

        var result = await new UpdateHubHandler(repository).Handle(new UpdateHub(hub.Id, "Roma", "1"), cancellationToken);

        result.Error.ShouldBe(HubErrors.InvalidZone);
        hub.Name.ShouldBe("Milano");
        hub.Zone.Code.ShouldBe("NORD");
        await repository.DidNotReceive().UpdateAsync(Arg.Any<Hub>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_reports_not_found_if_hub_disappears_before_persistence()
    {
        IHubRepository repository = Substitute.For<IHubRepository>();
        Hub hub = Hub.Create(Guid.NewGuid(), "Milano", "NORD").Value;
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        repository.GetByIdAsync(hub.Id, cancellationToken).Returns(hub);
        repository.UpdateAsync(hub, cancellationToken).Returns(false);

        var result = await new UpdateHubHandler(repository).Handle(new UpdateHub(hub.Id, "Roma", "SUD"), cancellationToken);

        result.Error.ShouldBe(HubErrors.NotFound(hub.Id));
        await repository.Received(1).UpdateAsync(hub, cancellationToken);
    }
}
