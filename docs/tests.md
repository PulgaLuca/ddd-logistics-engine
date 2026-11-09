# Test in LogisticsEngine

I test verificano proprietà diverse del sistema: suddividerli per responsabilità aiuta a capire dove cercare un difetto e mantiene rapida la suite quotidiana.

## Tipi di test e responsabilità

| Tipo | Cosa verifica | Dipendenze | Esempio nel progetto |
|---|---|---|---|
| Unit test | Una regola o una funzione isolata, con confini sostituiti da double | Nessuna rete o file reali | `LogisticsEngine.UnitTests/Domain/Hubs/HubTests.cs` verifica invarianti dell'entità Hub; `Application/Hubs/Validation/HubCommandValidatorsTests.cs` verifica la validazione dei command |
| Integration test | La collaborazione reale tra più componenti, per esempio HTTP, MediatR, handler e repository | Server di test in-process e storage temporaneo | `LogisticsEngine.IntegrationTests/ApiWorkflowTests.cs` chiama gli endpoint v1 e usa i repository JSON registrati dall'app |
| Architecture test | Regole strutturali tra progetti e layer | Assembly compilati | `LogisticsEngine.ArchitectureTests/LayerBoundaryTests.cs` impedisce dipendenze verso l'interno e controller collegati ai repository |
| Contract/API test | Compatibilità del contratto pubblico: rotte, codici, JSON e schema OpenAPI | Host HTTP o documento OpenAPI | Gli integration test controllano status e campi JSON; controlli dedicati allo schema OpenAPI possono essere aggiunti quando il contratto viene versionato |
| End-to-end (E2E) | Un flusso completo attraverso applicazione avviata e dipendenze esterne configurate come in un ambiente reale | Processi, rete e servizi esterni | Non è presente una suite E2E: gli integration test in-process sono più veloci e non avviano database o processi esterni |
| Manuale/esplorativo | Comportamenti difficili da codificare o controlli UX e operativi | Persona e ambiente | Provare Swagger UI in `Development` e invocare `GET /health` |

## Unit test: una regola per volta

Un test unitario prepara il minimo stato necessario, invoca il componente e controlla l'esito. Per esempio, `HubTests` può controllare che una zona non valida non crei un Hub; il test non deve avviare l'API né scrivere `backend/data`.

Per un validator FluentValidation si passa un command, si chiama `Validate` e si controllano gli errori e le proprietà coinvolte. Questo chiarisce se il problema è nella singola regola o nella sua integrazione con l'host.

## Integration test: dal client HTTP all'archivio

`ApiWorkflowTests` usa `WebApplicationFactory<Program>`: ASP.NET Core crea l'host nel processo di test e `HttpClient` invia richieste HTTP vere al middleware MVC. Non serve avviare Kestrel né occupare porte locali.

Il test configura `Storage:DataDirectory` con una directory temporanea unica. `Program` la legge e registra le implementazioni JSON reali tramite `AddInfrastructure`; il test non modifica i dati dello sviluppatore. Al termine chiude client e host e rimuove quella directory.

### Flusso Hub commentato passo per passo

1. **Preparazione:** `InitializeAsync` crea l'host `Testing`, configura lo storage temporaneo e crea il client HTTP.
2. **Invio:** `POST /api/v1/hubs` manda JSON con `name` e `zone`. Il model binding crea il contratto HTTP.
3. **Controller:** `HubsController` trasforma il payload in `RegisterHubCommand` e lo invia con `ISender`.
4. **Application e Domain:** MediatR individua handler e pipeline di validazione; le regole del Domain creano l'Hub.
5. **Infrastructure:** il repository JSON salva l'Hub nella directory isolata e l'API risponde `201 Created` con il DTO e `Location`.
6. **Lettura:** il test estrae l'ID dalla risposta e invia `GET /api/v1/hubs/{id}`. Controlla `200 OK`, nome e zona: così verifica anche che il dato sia recuperabile dopo il salvataggio.
7. **Pulizia:** `DisposeAsync` termina l'host e cancella solo la directory temporanea del test.

Altri due casi coprono il rifiuto di un Hub con nome vuoto (`400` con `ProblemDetails`) e la creazione/lettura di una spedizione. I commenti nel codice associano ogni passaggio agli elementi attraversati.

## Architecture test: regole sui confini

I test architetturali controllano dipendenze dell'assembly, quindi falliscono quando un cambiamento introduce una referenza proibita:

- Domain non dipende da Application, Infrastructure o API.
- Application non dipende da Infrastructure o API: riceve porte e richieste, non implementazioni concrete.
- Infrastructure non dipende da API: persistenza e integrazioni non conoscono controller o payload HTTP.
- I controller ricevono `ISender`, mantenendo il percorso verso i casi d'uso Mediatr.

Questi test non dimostrano la correttezza funzionale dei metodi: controllano che la struttura continui a rispettare la direzione delle dipendenze desiderata.

## Esecuzione

Dalla cartella `backend`:

```sh
dotnet test LogisticsEngine.slnx
```

Per iterare su una suite specifica:

```sh
dotnet test tests/LogisticsEngine.UnitTests
dotnet test tests/LogisticsEngine.ArchitectureTests
dotnet test tests/LogisticsEngine.IntegrationTests
```

Gli integration test non devono essere eseguiti in parallelo condividendo la cartella `backend/data`: ciascuna istanza usa il proprio percorso temporaneo. Mantenere i test unitari veloci, aggiungere test architetturali quando si stabilisce una regola strutturale, e usare integration test per i percorsi fra componenti che devono collaborare davvero.
