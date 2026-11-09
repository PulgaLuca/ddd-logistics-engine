# Prima bozza: utenti, autenticazione e autorizzazione

Questa prima iterazione aggiunge il modello e i casi d'uso per gli utenti. Lo storage iniziale è `users.json`; il repository rilegge il file a ogni operazione. In seguito il repository può essere sostituito con un database mantenendo le interfacce.

## Responsabilità dei layer

| Layer | Responsabilità | Elementi introdotti |
|---|---|---|
| Domain | Identità utente, ruoli, permessi e regole che associano ruoli a permessi | `User`, `UserName`, `PasswordHash`, `UserRole`, `UserPermission`, `IUserRepository` |
| Application | Casi d'uso e porte per le capacità esterne | `RegisterUserCommand`, `AuthenticateUserCommand`, `AuthorizeUserQuery`, `IPasswordHasher`, `IUserAuthenticationService`, `IUserAuthorizationService` |
| Infrastructure | Implementazione della persistenza e delle operazioni crittografiche | `JsonUserRepository`, `Pbkdf2PasswordHasher`, `JsonUserAuthenticationService`, `DomainUserAuthorizationService` |
| API | Espone registrazione e login pubblici, valida bearer token e protegge il controllo permessi | `UsersController`, handler Bearer e configurazione middleware |

`AddInfrastructure(dataDirectory)` registra le implementazioni concrete sulle interfacce. Application e Domain restano indipendenti da JSON e PBKDF2.

## Flusso di registrazione

1. `RegisterUserCommand` riceve nome utente e password per la sola durata del caso d'uso.
2. Il validator controlla il formato del nome e la lunghezza minima della password (12 caratteri); il handler controlla anche che il nome non esista già.
3. `IPasswordHasher.Hash` crea un hash PBKDF2 con salt casuale. Il salt e i parametri sono inclusi nel valore hash per permettere la verifica successiva.
4. Il Domain crea `User` con il `PasswordHash`, mai con la password originale. La registrazione assegna sempre il ruolo `Viewer`.
5. `IUserRepository.Add` salva il record in `users.json`. Il DTO di risposta non contiene né password né hash.

Esempio indicativo del documento (hash dimostrativo omesso):

```json
[
  {
    "id": "...",
    "userName": "mario.rossi",
    "passwordHash": "pbkdf2-sha256$600000$<salt-base64>$<hash-base64>",
    "role": "Viewer"
  }
]
```

## Flusso di autenticazione

1. `AuthenticateUserCommand` riceve nome utente e password.
2. L'handler inoltra la richiesta a `IUserAuthenticationService` senza conoscere il formato dell'archivio.
3. `JsonUserAuthenticationService` normalizza il nome, recupera l'utente attraverso `IUserRepository` (che legge `users.json`) e passa la password e l'hash a `IPasswordHasher.Verify`.
4. `Pbkdf2PasswordHasher` ricava l'hash usando salt e numero di iterazioni memorizzati e li confronta a tempo costante.
5. Nome inesistente e password errata producono lo stesso errore `User.InvalidCredentials`, evitando di indicare quale parte delle credenziali non corrisponde.

L'hash usa PBKDF2-HMAC-SHA256, salt casuale da 16 byte e 600.000 iterazioni. Il formato conserva algoritmo e parametri così da poter pianificare migrazioni future. Il numero di iterazioni accettato in lettura è limitato per rifiutare valori corrotti o deliberatamente eccessivi.

## Flusso di autorizzazione

1. `AuthorizeUserQuery` identifica l'utente e il `UserPermission` richiesto.
2. `IUserAuthorizationService` recupera l'utente dal repository.
3. `User.HasPermission` applica la policy del Domain: `Viewer` legge; `Operator` legge e gestisce Hub e Shipment; `Administrator` possiede tutti i permessi definiti.
4. Il risultato distingue utente mancante (`NotFound`) da permesso negato (`Forbidden`).

Il command di registrazione assegna solo `Viewer`: l'assegnazione di ruoli elevati richiederà in seguito un caso d'uso amministrativo protetto.

## Autenticazione HTTP

`POST /api/v1/users/authenticate` emette un token bearer firmato HMAC-SHA256, con scadenza di un'ora, dopo aver verificato le credenziali. L'handler ASP.NET Core valida la firma e la scadenza, poi costruisce i claim `NameIdentifier`, `Name` e `Role`. `GET /api/v1/users/me/permissions/{permission}` è protetto da `[Authorize]` e ricava l'ID dai claim verificati.

Non inviare mai l'ID utente come prova d'identità: chiunque potrebbe dichiarare l'ID di un altro account. Le decisioni HTTP possono usare policy e claim; il caso d'uso di autorizzazione rilegge il ruolo dal repository e applica la policy del Domain.

## Limiti dello storage JSON

`users.json` è un archivio didattico locale. Il lock evita scritture concorrenti tra istanze del repository nello stesso processo; non coordina processi multipli e il controllo unicità durante la registrazione non è una garanzia transazionale. Non conservare questo file in una cartella pubblica o in repository Git. La chiave di firma in `appsettings.Development.json` è solo dimostrativa: ogni ambiente deve configurare una chiave segreta protetta di almeno 32 byte. Un servizio reale dovrebbe usare storage protetto, backup, audit, rate limiting e procedure di recupero credenziali.
