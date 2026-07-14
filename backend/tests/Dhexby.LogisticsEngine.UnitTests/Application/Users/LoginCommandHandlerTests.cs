using Dhexby.LogisticsEngine.Application.Users;
using Dhexby.LogisticsEngine.Application.Users.Login;
using Dhexby.LogisticsEngine.Domain.Users;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.UnitTests.Application.Users;

public class LoginCommandHandlerTests
{
    private readonly InMemoryUserRepository _repository = new();
    private readonly FakePasswordHasher _hasher = new();
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _handler = new LoginCommandHandler(_repository, _hasher, new FakeJwtTokenGenerator());
    }

    private async Task SeedUserAsync(string email, string password)
    {
        User user = User.Create(
            Email.Create(email).Value,
            _hasher.Hash(password),
            UserRole.Operator,
            DateTime.UtcNow).Value;

        await _repository.AddAsync(user);
    }

    [Fact]
    public async Task Handle_ShouldReturnToken_WhenCredentialsValid()
    {
        await SeedUserAsync("user@example.com", "password123");

        Result<AuthResponse> result = await _handler.Handle(
            new LoginCommand("user@example.com", "password123"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Operator", result.Value.Role);
        Assert.False(string.IsNullOrEmpty(result.Value.AccessToken));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenPasswordWrong()
    {
        await SeedUserAsync("user@example.com", "password123");

        Result<AuthResponse> result = await _handler.Handle(
            new LoginCommand("user@example.com", "wrong-password"),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.InvalidCredentials, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenUserUnknown()
    {
        Result<AuthResponse> result = await _handler.Handle(
            new LoginCommand("nobody@example.com", "password123"),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.InvalidCredentials, result.Error);
    }
}
