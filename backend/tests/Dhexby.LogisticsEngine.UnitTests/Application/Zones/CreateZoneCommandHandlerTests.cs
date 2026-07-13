using Dhexby.LogisticsEngine.Application.Zones.CreateZone;
using Dhexby.LogisticsEngine.Domain.Zones;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.UnitTests.Application.Zones;

public class CreateZoneCommandHandlerTests
{
    private readonly InMemoryZoneRepository _repository = new();
    private readonly CreateZoneCommandHandler _handler;

    public CreateZoneCommandHandlerTests()
    {
        _handler = new CreateZoneCommandHandler(_repository);
    }

    [Fact]
    public async Task Handle_ShouldCreateRootZone()
    {
        Result<Guid> result = await _handler.Handle(
            new CreateZoneCommand("ZON-0001", "Lombardia", ParentZoneId: null),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Zone? saved = await _repository.GetByIdAsync(result.Value);
        Assert.NotNull(saved);
        Assert.Null(saved.ParentZoneId);
    }

    [Fact]
    public async Task Handle_ShouldCreateSubzone_WhenParentExists()
    {
        Guid parentId = (await _handler.Handle(
            new CreateZoneCommand("ZON-0001", "Lombardia", null),
            CancellationToken.None)).Value;

        Result<Guid> result = await _handler.Handle(
            new CreateZoneCommand("ZON-0002", "Milano", parentId),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Zone? subzone = await _repository.GetByIdAsync(result.Value);
        Assert.Equal(parentId, subzone!.ParentZoneId);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenParentDoesNotExist()
    {
        Guid missingParent = Guid.NewGuid();

        Result<Guid> result = await _handler.Handle(
            new CreateZoneCommand("ZON-0002", "Milano", missingParent),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ZoneErrors.ParentNotFound(missingParent), result.Error);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCodeDuplicated()
    {
        await _handler.Handle(new CreateZoneCommand("ZON-0001", "Lombardia", null), CancellationToken.None);

        Result<Guid> result = await _handler.Handle(
            new CreateZoneCommand("ZON-0001", "Altro", null),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ZoneErrors.CodeAlreadyInUse, result.Error);
    }
}
