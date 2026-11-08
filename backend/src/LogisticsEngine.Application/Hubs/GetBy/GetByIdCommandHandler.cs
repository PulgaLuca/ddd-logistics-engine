using FluentValidation;
using LogisticsEngine.Application.Hubs.GetById;
using LogisticsEngine.Application.Hubs.Validation;
using LogisticsEngine.Application.Shared;
using LogisticsEngine.Domain.Hubs;
using LogisticsEngine.Domain.Hubs.Entities;
using LogisticsEngine.Domain.Hubs.Errors;
using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Application.Hubs.GetBy;

/// <summary>Recupera un Hub tramite repository e restituisce un DTO o un errore tipizzato.</summary>
public sealed class GetByIdCommandHandler(IHubRepository repository, IValidator<GetByIdCommand> validator) : ICommandHandler<GetByIdCommand, Result<HubDto>>
{
    public Result<HubDto> Handle(GetByIdCommand command)
    {
        FluentValidation.Results.ValidationResult validation = validator.Validate(command);
        if (!validation.IsValid)
        {
            return Result.Failure<HubDto>(HubValidation.ToError(validation));
        }

        Hub? hub = repository.GetById(command.Id);
        return hub is null ? Result.Failure<HubDto>(HubErrors.NotFound(command.Id)) : Result.Success(HubDto.From(hub));
    }
}
