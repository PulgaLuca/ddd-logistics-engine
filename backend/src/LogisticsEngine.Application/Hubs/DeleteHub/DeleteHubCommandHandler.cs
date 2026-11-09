using FluentValidation;
using LogisticsEngine.Application.Hubs.Validation;
using LogisticsEngine.Domain.Hubs;
using LogisticsEngine.Domain.Hubs.Errors;
using LogisticsEngine.SharedKernel;
using MediatR;

namespace LogisticsEngine.Application.Hubs.DeleteHub;

/// <summary>Elimina un Hub o restituisce il suo errore di not found.</summary>
public sealed class DeleteHubCommandHandler(IHubRepository repository, IValidator<DeleteHubCommand> validator) : IRequestHandler<DeleteHubCommand, Result>
{
    public Task<Result> Handle(DeleteHubCommand command, CancellationToken cancellationToken)
    {
        FluentValidation.Results.ValidationResult validation = validator.Validate(command);
        if (!validation.IsValid)
        {
            return Task.FromResult(Result.Failure(HubValidation.ToError(validation)));
        }

        return Task.FromResult(repository.Delete(command.Id) ? Result.Success() : Result.Failure(HubErrors.NotFound(command.Id)));
    }
}
