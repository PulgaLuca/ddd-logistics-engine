using LogisticsEngine.Domain.Hubs.Errors;
using LogisticsEngine.Domain.Shared.Names;

namespace LogisticsEngine.UnitTests.Domain.Names;

public sealed class NameTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_rejects_empty_values(string? value) => Name.Create(value).Error.ShouldBe(NameErrors.InvalidName);

    [Fact]
    public void Create_trims_and_accepts_maximum_length()
    {
        Name.Create(" Milano ").Value.Value.ShouldBe("Milano");
        Name.Create(new string('a', 120)).IsSuccess.ShouldBeTrue();
        Name.Create(new string('a', 121)).Error.ShouldBe(NameErrors.InvalidName);
    }

    [Fact]
    public void Equality_uses_normalized_value()
    {
        Name first = Name.Create("Roma").Value;
        Name second = Name.Create(" Roma ").Value;
        first.Equals(second).ShouldBeTrue();
        first.GetHashCode().ShouldBe(second.GetHashCode());
        first.ToString().ShouldBe("Roma");
    }
}
