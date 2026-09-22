# Architettura

I riferimenti fra progetti puntano verso i layer interni:

```text
WebApi -> Application -> Domain -> SharedKernel
WebApi -> Infrastructure -> Application
```

WebApi è il punto di composizione: richiama AddApplication e AddInfrastructure.
Domain e SharedKernel non dipendono da framework web, database o MediatR.
Application contiene la pipeline FluentValidation e registra gli handler MediatR.
Infrastructure è pronta a ospitare implementazioni senza imporre database o servizi.

Per aggiungere un caso d'uso:
1. Inserisci entità e invarianti in Domain.
2. Definisci richiesta IRequest<T>, handler IRequestHandler<TRequest,TResponse> e AbstractValidator<TRequest> in Application.
3. Definisci le interfacce necessarie in Application e implementale in Infrastructure.
4. Registra le implementazioni in AddInfrastructure.
5. Esponi un endpoint WebApi che invia la richiesta con ISender.
6. Aggiungi test nei progetti già predisposti.

I test architetturali si eseguono con dotnet test; dotnet build compila ma non esegue i test.
Per creare un progetto nuovo, segui il README nella cartella principale.
