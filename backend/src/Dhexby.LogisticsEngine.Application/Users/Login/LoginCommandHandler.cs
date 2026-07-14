using Dhexby.LogisticsEngine.Application.Abstractions.Authentication;
using Dhexby.LogisticsEngine.Domain.Users;
using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Users.Login;

/// <summary>
/// Use case: verifica le credenziali e, se valide, emette il token. In caso di
/// errore restituisce sempre lo stesso messaggio generico (non rivela se sia
/// sbagliata l'email o la password).
/// </summary>
public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator tokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        Result<Email> emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
        {
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
        }

        User? user = await _userRepository.GetByEmailAsync(emailResult.Value, cancellationToken);
        if (user is null || !_passwordHasher.Verify(user.PasswordHash, request.Password))
        {
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
        }

        AccessToken token = _tokenGenerator.Generate(user);

        return new AuthResponse(token.Token, token.ExpiresAtUtc, user.Email.Value, user.Role.ToString());
    }
}
