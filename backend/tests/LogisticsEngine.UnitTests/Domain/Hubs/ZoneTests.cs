using LogisticsEngine.Domain.Hubs.Errors;
using LogisticsEngine.Domain.Hubs.Vo;

namespace LogisticsEngine.UnitTests.Domain.Hubs;

public sealed class ZoneTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("A")]
    [InlineData("ABCDEFGHIJK")]
    [InlineData("N0RD")]
    [InlineData("N ORD")]
    [InlineData("SÙD")]
    public void Create_rejects_invalid_codes(string? code) => Zone.Create(code).Error.ShouldBe(HubErrors.InvalidZone);

    [Theory]
    [InlineData("AB")]
    [InlineData("ABCDEFGHIJ")]
    [InlineData(" nord ")]
    public void Create_normalizes_and_accepts_boundary_lengths(string code)
    {
        Zone zone = Zone.Create(code).Value;
        zone.Code.ShouldBe(code.Trim().ToUpperInvariant());
        zone.ToString().ShouldBe(zone.Code);
    }

    [Fact]
    public void Equality_uses_code()
    {
        Zone first = Zone.Create("nord").Value;
        Zone second = Zone.Create("NORD").Value;
        first.Equals(second).ShouldBeTrue();
        first.GetHashCode().ShouldBe(second.GetHashCode());
        first.Equals(Zone.Create("SUD").Value).ShouldBeFalse();
    }
}
