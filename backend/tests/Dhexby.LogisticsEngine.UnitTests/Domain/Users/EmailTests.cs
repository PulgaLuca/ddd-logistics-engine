using Dhexby.LogisticsEngine.Domain.Users;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.UnitTests.Domain.Users;

public class EmailTests
{
    [Fact]
    public void Create_ShouldNormalizeToLowercase_AndTrim()
    {
        Result<Email> result = Email.Create("  Mario.Rossi@Example.COM ");

        Assert.True(result.IsSuccess);
        Assert.Equal("mario.rossi@example.com", result.Value.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldFail_WhenEmpty(string? value)
    {
        Result<Email> result = Email.Create(value);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.EmailEmpty, result.Error);
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("missing@dot")]
    [InlineData("@example.com")]
    [InlineData("spaces in@email.com")]
    public void Create_ShouldFail_WhenFormatInvalid(string value)
    {
        Result<Email> result = Email.Create(value);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.EmailInvalidFormat, result.Error);
    }

    [Fact]
    public void Equality_ShouldBeStructural()
    {
        Email first = Email.Create("user@example.com").Value;
        Email second = Email.Create("USER@example.com").Value;

        Assert.Equal(first, second);
    }
}
