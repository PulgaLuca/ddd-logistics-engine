using FluentValidation;
using LogisticsEngine.Application.Shared;
using LogisticsEngine.Application.Hubs.Validation;
using LogisticsEngine.Domain.Hubs;
using LogisticsEngine.Domain.Hubs.Errors;
using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Application.Hubs.DeleteHub;

/// <summary>Elimina un Hub o restituisce il suo errore di not found.</summary>
public sealed class DeleteHubCommandHandler(IHubRepository repository, IValidator<DeleteHubCommand> validator) : ICommandHandler<DeleteHubCommand, Result>
{
    public Result Handle(DeleteHubCommand command)
    {
        FluentValidation.Results.ValidationResult validation = validator.Validate(command);
        if (!validation.IsValid)
        {
            return Result.Failure(HubValidation.ToError(validation));
        }

        return repository.Delete(command.Id) ? Result.Success() : Result.Failure(HubErrors.NotFound(command.Id));
    }
}
