using FluentValidation;
using LogisticsEngine.Application.Hubs.GetById;
using LogisticsEngine.Application.Hubs.Validation;
using LogisticsEngine.Domain.Hubs;
using LogisticsEngine.Domain.Hubs.Entities;
using LogisticsEngine.Domain.Hubs.Errors;
using LogisticsEngine.SharedKernel;
using MediatR;

namespace LogisticsEngine.Application.Hubs.GetBy;

/// <summary>Recupera un Hub tramite repository e restituisce un DTO o un errore tipizzato.</summary>
public sealed class GetByIdCommandHandler(IHubRepository repository, IValidator<GetHubByIdQuery> validator) : IRequestHandler<GetHubByIdQuery, Result<HubDto>>
{
    public Task<Result<HubDto>> Handle(GetHubByIdQuery command, CancellationToken cancellationToken)
    {
        FluentValidation.Results.ValidationResult validation = validator.Validate(command);
        if (!validation.IsValid)
        {
            return Task.FromResult(Result.Failure<HubDto>(HubValidation.ToError(validation)));
        }

        Hub? hub = repository.GetById(command.Id);
        return Task.FromResult(hub is null ? Result.Failure<HubDto>(HubErrors.NotFound(command.Id)) : Result.Success(HubDto.From(hub)));
    }
}
