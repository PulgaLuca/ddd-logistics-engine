using Dhexby.LogisticsEngine.Application.Abstractions.Behaviors;
using Dhexby.SharedKernel;
using FluentValidation;
using MediatR;

namespace Dhexby.LogisticsEngine.UnitTests.Application.Behaviors;

public class ValidationBehaviorTests
{
    // Richiesta fittizia usata solo per esercitare la behavior.
    public sealed record SampleRequest(string Name) : IRequest<Result<Guid>>;

    private sealed class SampleRequestValidator : AbstractValidator<SampleRequest>
    {
        public SampleRequestValidator()
        {
            RuleFor(request => request.Name).NotEmpty();
        }
    }

    [Fact]
    public async Task Handle_ShouldCallNext_WhenNoValidatorsAreRegistered()
    {
        var behavior = new ValidationBehavior<SampleRequest, Result<Guid>>([]);
        Guid expected = Guid.NewGuid();

        Result<Guid> result = await behavior.Handle(
            new SampleRequest("whatever"),
            _ => Task.FromResult(Result.Success(expected)),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(expected, result.Value);
    }

    [Fact]
    public async Task Handle_ShouldCallNext_WhenValidationPasses()
    {
        var behavior = new ValidationBehavior<SampleRequest, Result<Guid>>([new SampleRequestValidator()]);

        Result<Guid> result = await behavior.Handle(
            new SampleRequest("valid"),
            _ => Task.FromResult(Result.Success(Guid.NewGuid())),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationFailure_AndNotCallNext_WhenValidationFails()
    {
        var behavior = new ValidationBehavior<SampleRequest, Result<Guid>>([new SampleRequestValidator()]);
        bool nextCalled = false;

        Result<Guid> result = await behavior.Handle(
            new SampleRequest(""),
            _ =>
            {
                nextCalled = true;
                return Task.FromResult(Result.Success(Guid.NewGuid()));
            },
            CancellationToken.None);

        Assert.False(nextCalled);
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
        ValidationError validationError = Assert.IsType<ValidationError>(result.Error);
        Assert.NotEmpty(validationError.Errors);
    }
}
