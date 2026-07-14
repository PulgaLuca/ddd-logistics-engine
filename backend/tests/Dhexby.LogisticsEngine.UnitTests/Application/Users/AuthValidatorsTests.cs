using Dhexby.LogisticsEngine.Application.Users.Login;
using Dhexby.LogisticsEngine.Application.Users.RegisterUser;
using FluentValidation.TestHelper;

namespace Dhexby.LogisticsEngine.UnitTests.Application.Users;

public class AuthValidatorsTests
{
    private readonly RegisterUserCommandValidator _registerValidator = new();
    private readonly LoginCommandValidator _loginValidator = new();

    [Fact]
    public void Register_ShouldPass_ForValidCommand()
    {
        var command = new RegisterUserCommand("user@example.com", "password123", "Viewer");

        _registerValidator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Register_ShouldFail_WhenPasswordTooShort()
    {
        var command = new RegisterUserCommand("user@example.com", "short", "Viewer");

        _registerValidator.TestValidate(command).ShouldHaveValidationErrorFor(command => command.Password);
    }

    [Fact]
    public void Register_ShouldFail_WhenRoleInvalid()
    {
        var command = new RegisterUserCommand("user@example.com", "password123", "Boss");

        _registerValidator.TestValidate(command).ShouldHaveValidationErrorFor(command => command.Role);
    }

    [Fact]
    public void Register_ShouldFail_WhenEmailInvalid()
    {
        var command = new RegisterUserCommand("nope", "password123", "Viewer");

        _registerValidator.TestValidate(command).ShouldHaveValidationErrorFor(command => command.Email);
    }

    [Fact]
    public void Login_ShouldFail_WhenFieldsEmpty()
    {
        var command = new LoginCommand("", "");

        TestValidationResult<LoginCommand> result = _loginValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(command => command.Email);
        result.ShouldHaveValidationErrorFor(command => command.Password);
    }
}
