using Dhexby.LogisticsEngine.Application.Abstractions.Authentication;
using Dhexby.LogisticsEngine.Domain.Users;
using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Users.RegisterUser;

/// <summary>
/// Use case: registra un nuovo utente con un ruolo, salva l'hash della password
/// e restituisce subito un token (così dopo la registrazione si è già loggati).
/// </summary>
public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator tokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<Result<AuthResponse>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        Result<Email> emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
        {
            return Result.Failure<AuthResponse>(emailResult.Error);
        }

        if (!Enum.TryParse(request.Role, ignoreCase: true, out UserRole role) || !Enum.IsDefined(role))
        {
            return Result.Failure<AuthResponse>(UserErrors.UnknownRole);
        }

        bool emailInUse = await _userRepository.ExistsByEmailAsync(emailResult.Value, cancellationToken);
        if (emailInUse)
        {
            return Result.Failure<AuthResponse>(UserErrors.EmailAlreadyInUse);
        }

        string passwordHash = _passwordHasher.Hash(request.Password);

        Result<User> userResult = User.Create(emailResult.Value, passwordHash, role, DateTime.UtcNow);
        if (userResult.IsFailure)
        {
            return Result.Failure<AuthResponse>(userResult.Error);
        }

        await _userRepository.AddAsync(userResult.Value, cancellationToken);

        AccessToken token = _tokenGenerator.Generate(userResult.Value);

        return new AuthResponse(
            token.Token,
            token.ExpiresAtUtc,
            userResult.Value.Email.Value,
            userResult.Value.Role.ToString());
    }
}
