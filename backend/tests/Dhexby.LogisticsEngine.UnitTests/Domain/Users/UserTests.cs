using Dhexby.LogisticsEngine.Domain.Users;
using Dhexby.LogisticsEngine.Domain.Users.Events;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.UnitTests.Domain.Users;

public class UserTests
{
    private static Email ValidEmail() => Email.Create("user@example.com").Value;

    [Fact]
    public void Create_ShouldSetRole_AndRaiseEvent()
    {
        Result<User> result = User.Create(ValidEmail(), "hashed-password", UserRole.Manager, DateTime.UtcNow);

        Assert.True(result.IsSuccess);
        Assert.Equal(UserRole.Manager, result.Value.Role);
        Assert.Contains(result.Value.DomainEvents, domainEvent =>
            domainEvent is UserRegisteredDomainEvent registered && registered.Role == UserRole.Manager);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_ShouldFail_WhenPasswordHashMissing(string? passwordHash)
    {
        Result<User> result = User.Create(ValidEmail(), passwordHash!, UserRole.Viewer, DateTime.UtcNow);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.InvalidCredentials, result.Error);
    }

    [Fact]
    public void Rehydrate_ShouldRestoreState_WithoutEvents()
    {
        User user = User.Rehydrate(Guid.NewGuid(), ValidEmail(), "hash", UserRole.Administrator, DateTime.UtcNow);

        Assert.Equal(UserRole.Administrator, user.Role);
        Assert.Empty(user.DomainEvents);
    }
}
