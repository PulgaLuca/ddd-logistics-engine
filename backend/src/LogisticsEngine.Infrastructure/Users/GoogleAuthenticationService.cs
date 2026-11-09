using System.Security.Cryptography;
using System.Text;
using Google.Apis.Auth;
using LogisticsEngine.Application.Users;
using LogisticsEngine.Domain.Users;
using LogisticsEngine.Domain.Users.Entities;
using LogisticsEngine.Domain.Users.Enums;
using LogisticsEngine.Domain.Users.ValueObjects;
using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Infrastructure.Users;

/// <summary>Verifica ID token Google e collega l'account a un'identità locale stabile basata sul subject Google.</summary>
public sealed class GoogleAuthenticationService(
    IUserRepository repository,
    IPasswordHasher passwordHasher,
    string clientId) : IGoogleAuthenticationService
{
    public async Task<GoogleAuthenticatedUser?> AuthenticateAsync(string? credential, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(credential) || string.IsNullOrWhiteSpace(clientId))
        {
            return null;
        }

        try
        {
            GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(
                credential, new GoogleJsonWebSignature.ValidationSettings { Audience = [clientId] });
            cancellationToken.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(payload.Subject) || payload.EmailVerified != true)
            {
                return null;
            }

            // Il subject Google, non l'email, identifica l'account per evitare collisioni o takeover di username.
            string key = "google-" + Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(payload.Subject)))[..56].ToLowerInvariant();
            Result<UserName> parsedName = UserName.Create(key);
            if (parsedName.IsFailure)
            {
                return null;
            }
            User? user = repository.GetByUserName(parsedName.Value);
            if (user is null)
            {
                Result<User> created = User.Create(Guid.NewGuid(), key,
                    passwordHasher.Hash(Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))), UserRole.Viewer,
                    payload.GivenName, payload.FamilyName);
                if (created.IsFailure)
                {
                    return null;
                }
                try { repository.Add(created.Value); user = created.Value; }
                catch (InvalidOperationException) { user = repository.GetByUserName(parsedName.Value); }
            }

            if (user is not null && string.IsNullOrWhiteSpace(user.FirstName) && string.IsNullOrWhiteSpace(user.LastName) &&
                (!string.IsNullOrWhiteSpace(payload.GivenName) || !string.IsNullOrWhiteSpace(payload.FamilyName)))
            {
                var profileResult = user.UpdateProfile(payload.GivenName, payload.FamilyName, user.ProfessionalRole);
                if (profileResult.IsSuccess) repository.Update(user);
            }

            string displayName = string.IsNullOrWhiteSpace(payload.Name) ? payload.Email ?? key : payload.Name!;
            return user is null ? null : new GoogleAuthenticatedUser(user, displayName ?? key);
        }
        catch (Exception)
        {
            return null;
        }
    }
}
