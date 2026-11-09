using LogisticsEngine.Application.Hubs.DeleteHub;
using LogisticsEngine.Application.Hubs.GetBy;
using LogisticsEngine.Application.Hubs.GetById;
using LogisticsEngine.Application.Hubs.RegisterHub;
using LogisticsEngine.Application.Hubs.UpdateHub;
using LogisticsEngine.Domain.Hubs.Errors;

namespace LogisticsEngine.UnitTests.Application.Hubs.Validation;

public sealed class HubCommandValidatorsTests
{
    [Fact]
    public void Register_validator_accepts_normalized_domain_values()
    {
        var result = new RegisterHubCommandValidator().Validate(new RegisterHubCommand(" Milano ", " nord "));

        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData(null, "NORD", "Hub.InvalidName")]
    [InlineData("Milano", null, "Hub.InvalidZone")]
    [InlineData("Milano", "N0RD", "Hub.InvalidZone")]
    public void Register_validator_reports_domain_error_codes(string? name, string? zone, string expectedCode)
    {
        var result = new RegisterHubCommandValidator().Validate(new RegisterHubCommand(name, zone));

        result.IsValid.ShouldBeFalse();
        result.Errors.Select(error => error.ErrorCode).ShouldContain(expectedCode);
    }

    [Fact]
    public void Update_validator_checks_id_and_values()
    {
        var result = new UpdateHubCommandValidator().Validate(new UpdateHubCommand(Guid.Empty, "", "X"));

        result.IsValid.ShouldBeFalse();
        result.Errors.Select(error => error.ErrorCode).ShouldContain(HubErrors.InvalidId.Code);
        result.Errors.Select(error => error.ErrorCode).ShouldContain(NameErrors.InvalidName.Code);
        result.Errors.Select(error => error.ErrorCode).ShouldContain(HubErrors.InvalidZone.Code);
    }

    [Fact]
    public void Delete_and_get_by_id_validators_reject_empty_ids()
    {
        new DeleteHubCommandValidator().Validate(new DeleteHubCommand(Guid.Empty)).Errors
            .Select(error => error.ErrorCode).ShouldContain(HubErrors.InvalidId.Code);
        new GetByIdCommandValidator().Validate(new GetHubByIdQuery(Guid.Empty)).Errors
            .Select(error => error.ErrorCode).ShouldContain(HubErrors.InvalidId.Code);
    }

    [Fact]
    public void Get_by_name_validator_applies_name_value_object_rules()
    {
        new GetByNameCommandValidator().Validate(new GetHubByNameQuery(new string('a', 121))).Errors
            .Select(error => error.ErrorCode).ShouldContain(NameErrors.InvalidName.Code);
        new GetByNameCommandValidator().Validate(new GetHubByNameQuery("Roma")).IsValid.ShouldBeTrue();
    }
}
