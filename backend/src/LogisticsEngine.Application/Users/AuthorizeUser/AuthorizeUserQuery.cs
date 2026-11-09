using LogisticsEngine.Domain.Users.Enums;
using LogisticsEngine.SharedKernel;
using MediatR;

namespace LogisticsEngine.Application.Users.AuthorizeUser;

/// <summary>Controlla se l'utente autenticato possiede uno specifico permesso.</summary>
public sealed record AuthorizeUserQuery(Guid UserId, UserPermission Permission) : IRequest<Result<bool>>;
