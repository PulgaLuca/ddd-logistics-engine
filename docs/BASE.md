# Percorso BASE — dal gestionale al motore logistico

## Obiettivo e metodo

Questo percorso porta il progetto da una semplice gestione di Hub a un piccolo motore logistico utilizzabile da più interfacce. Gli stessi argomenti sono ripresi nel percorso [ADVANCED.md](ADVANCED.md), con requisiti di robustezza, volume e operatività più elevati.

Il percorso parte dalla soluzione .NET in `backend/`: Domain, Application, Infrastructure, Console, WebAPI e test unitari. Prima di ogni tappa si registra il comportamento corrente con una build e test mirati; il refactoring deve lasciare il progetto avviabile. Le tappe possono essere svolte come PR piccole e successive.

## Due filoni funzionali

| Gestionale | Motore logistico |
|---|---|
| Amministra Hub, zone, corrieri e spedizioni. | Calcola assegnazioni, percorsi e priorità operative. |
| CRUD, ricerca, filtri e report. | Regole su capacità, connettività e tempi. |
| Esempio: rinominare l’Hub “Milano”. | Esempio: stabilire se una spedizione può raggiungere l’Hub “Roma”. |

I due filoni condividono le entità, ma non necessariamente le stesse applicazioni: una modifica anagrafica non dovrebbe attivare accidentalmente una ricalcolazione completa dei percorsi.

## Piano passo-passo

### 0. Baseline e mappa dell’applicazione

**Perché:** prima di modificare i confini bisogna sapere dove risiedono regole, persistenza e presentazione.

- Disegnare il flusso Console e WebAPI → Application → Domain → repository → JSON.
- Elencare i casi d’uso esistenti: registrazione, lettura, modifica e cancellazione Hub.
- Distinguere gli errori attesi (`Result`) dagli errori tecnici inattesi (per esempio file JSON corrotto).
- Conservare esempi riproducibili dei dati JSON e delle richieste API.

**Esempio:** seguire `POST /api/hubs` fino a `Hub.Create` e alla scrittura del documento JSON.

### 1. Due filoni: gestionale e motore logistico

**Perché:** CRUD e calcolo logistico hanno ritmi di evoluzione e responsabilità differenti.

- Tenere nel gestionale i casi d’uso di amministrazione.
- Introdurre nel motore un caso d’uso di pianificazione che usa Hub e Shipment senza occuparsi di menu o HTTP.
- Usare termini distinti: `RegisterHub` per l’anagrafica, `PlanShipment` per l’operatività.

**Esempio:** `RegisterHubCommand("Torino", "NORD")` aggiorna i dati; `FindRouteQuery(shipmentId)` calcola un percorso senza cambiare l’anagrafica.

### 2. Validazione Application con FluentValidation

**Perché:** la validazione dell’input offre errori comprensibili prima di avviare il caso d’uso; il Domain continua a proteggere le invarianti anche se il chiamante non passa da FluentValidation.

- Definire validator per `RegisterHubCommand` e `UpdateHubCommand`.
- Restituire gli errori di validazione come errori applicativi leggibili.
- Non spostare nel validator le invarianti essenziali dell’entità: `HubName.Create` e `Zone.Create` restano autorevoli.

```csharp
RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
RuleFor(x => x.Zone).NotEmpty().Length(2, 10).Matches("^[A-Za-z]+$");
```

**Esempio didattico:** mostrare separatamente un nome mancante, un nome troppo lungo e una zona valida dopo normalizzazione.

### 3. Casi d’uso, command e handler

**Perché:** una classe per caso d’uso rende espliciti input, controlli, regole invocate e risultato.

- Organizzare `Application/Hubs/GetAll`, `GetById`, `RegisterHub`, `UpdateHub` e `DeleteHub`.
- Ogni cartella contiene command, handler e, dove serve, validator.
- Per la prima demo gli handler possono restare sincroni con un’interfaccia `ICommandHandler<TCommand,TResult>`.
- Il controller dipende da interfacce e non costruisce handler o servizi concreti.

**Esempio:** `RegisterHubCommandHandler.Handle` valida, applica la regola di unicità, invoca la factory Domain e persiste il nuovo Hub.

### 4. Presentation Layer: Console e WebAPI

**Perché:** la presentazione raccoglie input e rende il risultato nel formato adatto all’utente; non contiene regole di business.

- Console: menu, lettura dei valori e messaggi, delegati ai casi d’uso.
- WebAPI: rotte HTTP, binding JSON, status code e Problem Details.
- Entrambe le interfacce possono chiamare gli stessi command handler.
- `Program.cs` è il composition root: configura `IServiceCollection`, repository, Application e controller.
- Spostare percorsi, nome file dati e modalità di avvio in `appsettings.json` e leggere la configurazione al bootstrap.

**Esempi:** input console `Nome: Torino`; `POST /api/hubs` con JSON. In entrambi i casi il Domain riceve gli stessi dati normalizzati.

### 5. Primi test d’integrazione

**Perché:** un test unitario verifica una classe isolata, mentre un test d’integrazione controlla che presentazione, Application, repository e serializzazione collaborino davvero.

- Usare `WebApplicationFactory` per un test HTTP essenziale.
- Assegnare al test una cartella dati temporanea, mai quella di sviluppo.
- Verificare registrazione, lettura e una risposta di validazione o not found.
- Lasciare i test unitari per Value Object, entità e algoritmi puri.

**Esempio:** POST un Hub, prendere l’ID dalla risposta, fare GET e verificare nome e zona.

### 6. Collezioni .NET, delegati e LINQ

**Perché:** scegliere una collezione per le operazioni richieste evita strutture inutilmente complesse.

- `List<T>` per mantenere una sequenza e scorrerla.
- `Stack<T>` per operazioni LIFO; `Queue<T>` per FIFO.
- `Dictionary<TKey,TValue>` per accesso per chiave; `HashSet<T>` per appartenenza e unicità.
- `Func<T,TResult>` trasforma o calcola; `Action<T>` esegue un’azione; `Predicate<T>` risponde sì/no.
- LINQ: confronto tra sintassi a metodi e sintassi query, `Where`, `Select`, `OrderBy`, `GroupBy`, `Aggregate`, `Join`.
- Mostrare l’esecuzione differita: la query viene valutata quando enumerata; `ToList()` materializza.

```csharp
Func<Hub, string> zoneOf = hub => hub.Zone.Code;
IEnumerable<Hub> north = hubs.Where(hub => zoneOf(hub) == "NORD"); // differita
Hub[] result = north.OrderBy(hub => hub.Name).ToArray();            // eseguita qui
```

**Esempi:** `Queue<Shipment>` per la coda di lavorazione, `Stack<Hub>` per mostrare una visita DFS iterativa, `GroupBy` per il report del numero di Hub per zona.

### 7. DSA e analisi della complessità

**Perché:** gli algoritmi spiegano il costo operativo del motore logistico e rendono la scelta della struttura dati motivata.

- Notazione asintotica `O`, caso migliore, medio e peggiore; costo in tempo e spazio.
- Liste e pile/code: inserimento, rimozione e accesso; esempi piccoli con `Queue<T>` e `Stack<T>`.
- Dizionari/tabelle hash: funzione di hash, bucket e collisioni; spiegare che il costo medio di lookup è atteso O(1), ma può peggiorare con molte collisioni.
- Insiemi: controllo di presenza ed eliminazione dei duplicati.
- Alberi binari di ricerca: radice, nodo, foglia, altezza; visite inorder, preorder e postorder.
- Grafi: lista di adiacenza e matrice; BFS con coda, DFS con pila o ricorsione, cenno ai cammini minimi non pesati.
- Confrontare strutture rispetto all’operazione prevalente, non scegliere solo in base alla familiarità.

| Operazione | Struttura adatta | Costo indicativo |
|---|---|---|
| Inserire in coda e processare in ordine | `Queue<T>` | enqueue/dequeue O(1) ammortizzato |
| Cercare rapidamente per ID | `Dictionary<Guid, Hub>` | O(1) medio, O(n) peggiore |
| Evitare zone duplicate | `HashSet<string>` | O(1) medio, O(n) peggiore |
| Visitare gli Hub raggiungibili | lista di adiacenza + BFS | O(V + E) tempo, O(V) spazio |
| Ordinare gli ID | sort integrato / Selection Sort didattico | O(n log n) medio tipico / O(n²) |

Per BST non bilanciato la ricerca è O(log n) medio su dati distribuiti, O(n) nel peggiore. L’altezza determina il costo; un albero sbilanciato non garantisce logaritmicità.

**Applicazione al progetto:** ordinare gli Hub per ID; raggruppare per zona con dizionario; trovare un percorso non pesato tra Hub con lista di adiacenza e BFS.

### 8. Persistenza: documentale, BSON e NoSQL

**Perché:** un formato documentale introduce un modello più vicino ai dati aggregati del dominio e chiarisce le differenze tra file JSON e database.

- Concetti: database, collection, document; confronto JSON/BSON (BSON è una rappresentazione binaria con tipi aggiuntivi, non un sinonimo di JSON).
- CRUD, filtri, proiezioni, ordinamento, indici e pipeline di aggregazione.
- Modellare i dati scegliendo tra incorporare dettagli piccoli e possedere i dati insieme, oppure referenziare documenti condivisi o grandi.
- Confrontare i file JSON correnti con una collection `hubs` e una `shipments`.

**Esempi:** filtro `{ zone: "NORD" }`; proiezione dei soli `name` e `zone`; indice sul campo `trackingNumber`; pipeline `$match` per zona seguita da `$group` per conteggio.

**Modellazione:** incorporare un indirizzo immutabile e breve nello Shipment; referenziare Hub quando molti Shipment condividono lo stesso Hub e l’Hub viene aggiornato indipendentemente.

### 9. Supabase e completamento

**Perché:** un backend remoto permette di confrontare un adattatore reale con il repository JSON locale.

- Aggiungere un’implementazione Infrastructure dietro un’interfaccia repository; Application e Domain non conoscono Supabase.
- Configurare URL e chiavi tramite secret/configurazione ambiente, mai in codice o repository.
- Creare tabelle semplici `hubs` e `shipments`, vincoli essenziali e operazioni CRUD.
- Provare l’accesso da un caso d’uso e descrivere come gestire timeout/errori remoti.

**Esempio:** sostituire la registrazione DI di `JsonHubRepository` con il repository Supabase lasciando invariati `GetById` e `RegisterHub`.
