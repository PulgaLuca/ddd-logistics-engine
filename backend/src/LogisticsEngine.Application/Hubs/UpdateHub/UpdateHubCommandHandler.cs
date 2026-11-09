using FluentValidation;
using LogisticsEngine.Application.Hubs.Validation;
using LogisticsEngine.Domain.Hubs;
using LogisticsEngine.Domain.Hubs.Entities;
using LogisticsEngine.Domain.Hubs.Errors;
using LogisticsEngine.Domain.Hubs.Services;
using LogisticsEngine.SharedKernel;
using MediatR;

namespace LogisticsEngine.Application.Hubs.UpdateHub;

/// <summary>Valida e applica una modifica, preservando i dati quando una regola fallisce.</summary>
public sealed class UpdateHubCommandHandler(IHubRepository repository, IHubDomainService domainService, IValidator<UpdateHubCommand> validator) : IRequestHandler<UpdateHubCommand, Result>
{
    public Task<Result> Handle(UpdateHubCommand command, CancellationToken cancellationToken)
    {
        FluentValidation.Results.ValidationResult validation = validator.Validate(command);
        if (!validation.IsValid)
        {
            return Task.FromResult(Result.Failure(HubValidation.ToError(validation)));
        }

        Hub? hub = repository.GetById(command.Id);
        if (hub is null)
        {
            return Task.FromResult(Result.Failure(HubErrors.NotFound(command.Id)));
        }

        Result uniqueName = domainService.EnsureNameIsUnique(command.Name, command.Id);
        if (uniqueName.IsFailure)
        {
            return Task.FromResult(uniqueName);
        }

        Result updated = hub.Update(command.Name, command.Zone);
        if (updated.IsFailure)
        {
            return Task.FromResult(updated);
        }

        return Task.FromResult(repository.Update(hub) ? Result.Success() : Result.Failure(HubErrors.NotFound(command.Id)));
    }
}
