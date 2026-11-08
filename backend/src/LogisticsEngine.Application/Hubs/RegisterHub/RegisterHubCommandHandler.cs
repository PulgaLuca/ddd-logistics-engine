using FluentValidation;
using LogisticsEngine.Application.Shared;
using LogisticsEngine.Application.Hubs.Validation;
using LogisticsEngine.Domain.Hubs;
using LogisticsEngine.Domain.Hubs.Entities;
using LogisticsEngine.Domain.Hubs.Services;
using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Application.Hubs.RegisterHub;

/// <summary>Valida, applica le regole cross-entity e persiste un nuovo Hub.</summary>
public sealed class RegisterHubCommandHandler(IHubRepository repository, IHubDomainService domainService, IValidator<RegisterHubCommand> validator) : ICommandHandler<RegisterHubCommand, Result<HubDto>>
{
    public Result<HubDto> Handle(RegisterHubCommand command)
    {
        FluentValidation.Results.ValidationResult validation = validator.Validate(command);
        if (!validation.IsValid)
        {
            return Result.Failure<HubDto>(HubValidation.ToError(validation));
        }

        Result uniqueName = domainService.EnsureNameIsUnique(command.Name);
        if (uniqueName.IsFailure)
        {
            return Result.Failure<HubDto>(uniqueName.Error);
        }

        Result<Hub> hub = Hub.Create(Guid.NewGuid(), command.Name, command.Zone);
        if (hub.IsFailure)
        {
            return Result.Failure<HubDto>(hub.Error);
        }

        repository.Add(hub.Value);
        return Result.Success(HubDto.From(hub.Value));
    }
}
