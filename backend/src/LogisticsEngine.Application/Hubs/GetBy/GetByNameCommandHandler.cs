using FluentValidation;
using LogisticsEngine.Application.Hubs.Validation;
using LogisticsEngine.Domain.Hubs;
using LogisticsEngine.Domain.Hubs.Entities;
using LogisticsEngine.Domain.Hubs.Errors;
using LogisticsEngine.Domain.Shared.Names;
using LogisticsEngine.SharedKernel;
using MediatR;

namespace LogisticsEngine.Application.Hubs.GetBy;

/// <summary>Recupera un Hub tramite repository e restituisce un DTO o un errore tipizzato.</summary>
public sealed class GetByNameCommandHandler(IHubRepository repository, IValidator<GetHubByNameQuery> validator) : IRequestHandler<GetHubByNameQuery, Result<HubDto>>
{
    public Task<Result<HubDto>> Handle(GetHubByNameQuery command, CancellationToken cancellationToken)
    {
        FluentValidation.Results.ValidationResult validation = validator.Validate(command);
        if (!validation.IsValid)
        {
            return Task.FromResult(Result.Failure<HubDto>(HubValidation.ToError(validation)));
        }

        Result<Name> newName = Name.Create(command.Name);
        if (newName.IsFailure)
        {
            return Task.FromResult(Result.Failure<HubDto>(newName.Error));
        }

        Hub? hub = repository.GetByName(newName.Value);
        return Task.FromResult(hub is null ? Result.Failure<HubDto>(HubErrors.NotFoundByName(newName.Value.Value)) : Result.Success(HubDto.From(hub)));
    }
}
