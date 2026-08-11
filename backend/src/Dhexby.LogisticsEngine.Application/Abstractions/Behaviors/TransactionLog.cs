using Microsoft.Extensions.Logging;

namespace Dhexby.LogisticsEngine.Application.Abstractions.Behaviors;

/// <summary>
/// Messaggi di log del <see cref="TransactionBehavior{TRequest, TResponse}"/>,
/// generati a compile time dal source generator di LoggerMessage: nessuna
/// allocazione e nessuna formattazione se il livello non è abilitato.
/// </summary>
internal static partial class TransactionLog
{
    [LoggerMessage(
        EventId = 1000,
        Level = LogLevel.Information,
        Message = "Rollback della transazione per {Request}: {ErrorCode}")]
    public static partial void RollingBack(ILogger logger, string request, string errorCode);
}
