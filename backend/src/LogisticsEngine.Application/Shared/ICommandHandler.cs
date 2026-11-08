namespace LogisticsEngine.Application.Shared;

/// <summary>Contratto sincrono per un caso d'uso che riceve un comando e produce una risposta (risultato con incapsulamento risposta).</summary>
public interface ICommandHandler<in TCommand, out TResult>
{
    TResult Handle(TCommand command);
}
