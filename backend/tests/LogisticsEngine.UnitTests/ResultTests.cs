using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.UnitTests;

public sealed class ResultTests
{
    [Fact]
    public void Success_has_no_error_and_exposes_value()
    {
        Result.Success().IsSuccess.ShouldBeTrue();
        Result.Success().IsFailure.ShouldBeFalse();
        Result.Success().Error.ShouldBe(Error.None);
        Result.Success(42).Value.ShouldBe(42);
    }

    [Fact]
    public void Failure_exposes_error_but_never_value()
    {
        Error error = Error.Validation("Invalid", "Invalid value");
        Result.Failure(error).IsFailure.ShouldBeTrue();
        Result<int> result = Result.Failure<int>(error);
        result.Error.ShouldBe(error);
        Should.Throw<InvalidOperationException>(() => result.Value);
    }

    [Fact]
    public void Invalid_results_are_rejected()
    {
        Should.Throw<ArgumentException>(() => Result.Failure(Error.None));
        Should.Throw<ArgumentException>(() => Result.Failure<int>(Error.None));
        Should.Throw<ArgumentNullException>(() => Result.Success<string>(null!));
        Should.Throw<ArgumentNullException>(() => Result.Failure(null!));
    }

    [Fact]
    public void Errors_have_value_equality_and_type()
    {
        Error.Validation("Code", "Description").ShouldBe(Error.Validation("Code", "Description"));
        Error.Validation("Code", "Description").Type.ShouldBe(ErrorType.Validation);
        Error.NotFound("Missing", "Missing hub").Type.ShouldBe(ErrorType.NotFound);
    }
}
