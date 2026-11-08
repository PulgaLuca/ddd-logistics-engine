using LogisticsEngine.Application.Shared;
using LogisticsEngine.Domain.Hubs;
using LogisticsEngine.Domain.Hubs.Entities;
using LogisticsEngine.Domain.Hubs.Errors;
using LogisticsEngine.Domain.Shared.Names;
using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Application.Hubs.GetBy;

/// <summary>Recupera un Hub tramite repository e restituisce un DTO o un errore tipizzato.</summary>
public sealed class GetByNameCommandHandler(IHubRepository repository, IValidator<GetByNameCommand> validator) : ICommandHandler<GetByNameCommand, Result<HubDto>>
{
    public Result<HubDto> Handle(GetByNameCommand command)
    {
        FluentValidation.Results.ValidationResult validation = validator.Validate(command);
        if (!validation.IsValid)
        {
            return Result.Failure<HubDto>(HubValidation.ToError(validation));
        }

        Result<Name> newName = Name.Create(command.name);
        if (newName.IsFailure)
        {
            return Result.Failure<HubDto>(newName.Error);
        }

        Hub? hub = repository.GetByName(newName.Value);
        return hub is null ? Result.Failure<HubDto>(NameErrors.InvalidName) : Result.Success(HubDto.From(hub));
    }
}
using FluentValidation;
using LogisticsEngine.Application.Hubs.Validation;
