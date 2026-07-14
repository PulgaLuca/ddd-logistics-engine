using Dhexby.LogisticsEngine.Application.Users;
using Dhexby.LogisticsEngine.Application.Users.RegisterUser;
using Dhexby.LogisticsEngine.Domain.Users;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.UnitTests.Application.Users;

public class RegisterUserCommandHandlerTests
{
    private readonly InMemoryUserRepository _repository = new();
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserCommandHandlerTests()
    {
        _handler = new RegisterUserCommandHandler(_repository, new FakePasswordHasher(), new FakeJwtTokenGenerator());
    }

    private static RegisterUserCommand ValidCommand(string email = "user@example.com", string role = "Manager") =>
        new(email, "password123", role);

    [Fact]
    public async Task Handle_ShouldPersistUser_HashPassword_AndReturnToken()
    {
        Result<AuthResponse> result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Manager", result.Value.Role);
        Assert.False(string.IsNullOrEmpty(result.Value.AccessToken));

        User? saved = await _repository.GetByEmailAsync(Email.Create("user@example.com").Value);
        Assert.NotNull(saved);
        Assert.Equal("hashed:password123", saved.PasswordHash); // salvato hashato, mai in chiaro
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenEmailAlreadyInUse()
    {
        await _handler.Handle(ValidCommand(), CancellationToken.None);

        Result<AuthResponse> result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.EmailAlreadyInUse, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenRoleUnknown()
    {
        Result<AuthResponse> result = await _handler.Handle(
            ValidCommand(role: "SuperUser"),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.UnknownRole, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenEmailInvalid()
    {
        Result<AuthResponse> result = await _handler.Handle(
            ValidCommand(email: "not-an-email"),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.EmailInvalidFormat, result.Error);
    }
}
