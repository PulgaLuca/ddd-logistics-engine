# Percorso ADVANCED — dal gestionale al motore logistico scalabile

## Obiettivo e impostazione

Questo percorso copre gli stessi argomenti del [percorso BASE](BASE.md), ma aumenta i requisiti di dominio, volume, affidabilità e operatività. Il risultato è un gestionale separato da un motore logistico che può pianificare e aggiornare spedizioni attraverso interfacce e persistenza sostituibili.


## Due filoni e confini

| Gestionale | Motore logistico |
|---|---|
| Gestisce configurazioni, Hub, operatori e anagrafiche. | Pianifica instradamento, capacità, finestre temporali e priorità. |
| privilegia tracciabilità, autorizzazioni, modifiche controllate e audit. | privilegia throughput, consistenza operativa, algoritmi e resilienza. |
| Può aggiornare un Hub senza ricalcolare tutti gli Shipment. | Reagisce alla modifica con un evento/command mirato e una pianificazione versionata. |

## Piano di refactoring progressivo


### 1. Gestionale e motore logistico come moduli distinti

**Perché:** Microservizi futuri in cui un portale unico usa un **servizio di logistica** e un portale come **gestionale** di può collegare ad 

- Definire moduli `HubManagement` e `LogisticsPlanning` con API applicative esplicite.
- `RegisterHub` aggiorna il modello amministrativo; `PlanShipment` riceve uno snapshot/versione della rete.
- Modellare la modifica di rete come evento interno con versione, evitando una dipendenza diretta dal controller del gestionale.
- Valutare processi separati solo dopo avere identificato un requisito di scaling o disponibilità.


### 2. Validazione stratificata con FluentValidation

**Perché:** i controlli di forma appartengono al bordo applicativo; invarianti e autorizzazioni richiedono livelli diversi e non vanno confusi.

- Validator per command/query, regole condizionali e composizione di regole riusabili.
- Convertire gli errori in una struttura applicativa stabile con campo, codice e messaggio; il Domain conserva i propri codici Result.
- Distinguere validazione sintattica, validazione di dominio, autorizzazione e conflitto di concorrenza.

```csharp
RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
RuleFor(x => x.Zone).MustAsync(zoneDirectory.ExistsAsync)
    .When(x => !string.IsNullOrWhiteSpace(x.Zone));
```

### 3. Command/query e handler

**Perché:** i casi d’uso diventano unità osservabili con dipendenze definite e politiche cross-cutting applicabili.

- Separare cartelle per operazione e request/response dedicate; query non modificano stato, command hanno effetti espliciti.
- La demo può mantenere handler sincroni; nel passaggio a I/O reale, introdurre API asincrone con `CancellationToken` e aggiornare i contratti in modo coerente.
- Usare pipeline per validazione, logging e misurazione solo quando il modello asincrono e la libreria scelta sono compatibili.


### 4. Presentation layer, API e configurazione

**Perché:** HTTP e console sono adattatori; devono gestire protocolli e presentazione, non orchestrare regole del motore.

- Spiegare il Presentation Layer come traduttore tra protocollo esterno e Application.
- API: contratti versionati, paginazione, Problem Details coerenti, idempotenza delle operazioni ritentabili.
- Console: configurazione da `appsettings.json`, override ambiente, input robusto, output leggibile; usa solo interfacce/casi d’uso.
- Composition root distinto per host; configurazione fortemente tipizzata e validata in avvio.
- Separare health check di processo da readiness delle dipendenze esterne.

**Esempio:** Console e WebAPI chiamano `IRegisterHubHandler`; l’API converte Result in 201/400/409, la console mostra un messaggio e il codice errore.

### 5. Strategia di test e primi test d’integrazione

**Perché:** integrazioni reali introducono serializzazione, configurazione, query e transazioni, non rilevabili con soli mock.

- Test HTTP con `WebApplicationFactory` e database/container isolato o adapter deterministico.
- Test repository contro lo storage effettivo per filtri, indici, mapping e transazioni.
- Test di contratto per interfacce repository e adapter remoti.
- Test end-to-end limitati ai flussi critici: registrazione Hub → aggiornamento rete → pianificazione Shipment.
- Test di concorrenza per unicità e aggiornamento di versioni, test di resilienza per timeout.

**Esempio:** due registrazioni con nome equivalente in parallelo: una sola deve riuscire grazie a vincolo atomico storage, l’altra riceve un conflitto stabile.

### 6. Collezioni .NET, Func/Action/Predicate e LINQ

**Perché:** le astrazioni generiche consentono strategie riusabili, ma enumerazioni multiple e closure possono aggiungere costo non evidente.

- Modellare `List<T>`, `Stack<T>`, `Queue<T>`, `Dictionary<TKey,TValue>` e `HashSet<T>` in termini di invarianti e workload.
- Delegati: `Func<T,TResult>` per comparatori/strategie di scoring, `Action<T>` per callback di visita, `Predicate<T>` per filtri.
- LINQ a metodi e query syntax; filtro, proiezione, ordinamento, grouping, aggregazione e join.
- Mostrare deferred execution, re-enumeration, materializzazione, side effect e differenza tra `IEnumerable<T>` e snapshot materializzato.
- Profilare allocazioni e non sostituire LINQ con loop manuali senza un dato che giustifichi la complessità aggiuntiva.

```csharp
Func<Route, decimal> score = route => route.Cost + route.EstimatedMinutes * timeWeight;
var ranked = routes.OrderBy(score).ThenBy(route => route.HubIds.Count).ToArray();
```

**Esempio:** `Queue<PlanningJob>` per lavoro FIFO, `PriorityQueue<Hub, decimal>` per espansione best-first, `HashSet<Guid>` per nodi visitati.

### 7. DSA, complessità e scelta in base al workload

**Perché:** nel motore la struttura scelta impatta direttamente throughput, memoria e tempi massimi.

- Per ogni algoritmo annotare costo temporale e spaziale, casi migliore/medio/peggiore e ipotesi sulla distribuzione dei dati.
- `O(1)`, `O(log n)`, `O(n)`, `O(n log n)`, `O(n²)`; distinguere worst-case da costo atteso e ammortizzato.
- Tabelle hash/dizionari: hash code, bucket, collisioni, chaining/open addressing a livello concettuale, resize e vulnerabilità a input avversari.
- Alberi: terminologia, BST, inorder/preorder/postorder, sbilanciamento; confrontare con alberi bilanciati e indici storage.
- Grafi: lista di adiacenza O(V+E) spazio, matrice O(V²); BFS/DFS, ricostruzione del percorso, Dijkstra per pesi non negativi, limiti del caso scelto.
- Selezionare struttura in base alle operazioni dominanti e alla densità del grafo; validare con benchmark.

| Problema | Struttura | Costo atteso / limite |
|---|---|---|
| Lookup Hub per ID | Dictionary/hash index | O(1) medio, O(n) worst-case astratto |
| Grafo sparso con BFS | Liste di adiacenza + Queue + HashSet | O(V+E) tempo, O(V+E) spazio |
| Grafo denso piccolo | Matrice di adiacenza | O(V²) spazio, verifica arco O(1) |
| Cammino minimo pesato non negativo | Adiacenza + PriorityQueue | tipicamente O((V+E) log V) |
| BST non bilanciato | Albero di ricerca | O(log n) medio assunto, O(n) worst-case |

**Applicazione:** sostituire matrice didattica con lista di adiacenza quando rete è sparsa; aggiungere BFS per percorso minimo non pesato e Dijkstra soltanto dopo aver definito unità e vincoli dei pesi.

### 8. Persistence documentale, BSON e NoSQL

**Perché:** document DB può adattarsi a snapshot e aggregati, ma query, indici e relazioni hanno costi e limiti propri.

- Spiegare database, collection e document; BSON binario e tipi come date/decimal/binary rispetto al JSON testuale.
- CRUD, filtri composti, proiezioni, sort, paginazione, indici singoli/composti/unique, pipeline di aggregazione.
- Modellare incorporamento e riferimento in base a ownership, frequenza di lettura, dimensione, aggiornamenti e consistenza.
- Definire indici a partire da query osservate; misurare piani di esecuzione e costo di scrittura.
- Valutare confini transazionali, idempotenza, schema evolution e strategie di migrazione.

**Esempi:** indice composto `{ zone: 1, status: 1 }` per filtro e ordinamento operativo; pipeline `$match` → `$group` → `$sort` per volumi per zona.

**Modellazione:** incorporare le tappe immutabili del piano come snapshot storico nello Shipment; referenziare Hub correnti per aggiornamenti amministrativi. Evitare di duplicare dati mutabili senza una politica di sincronizzazione.

### 9. Supabase come adapter e integrazione di sistema

**Perché:** l’integrazione remota rende visibili rete, segreti, autorizzazione, latenza e consistenza.

- Implementare adapter in Infrastructure dietro porte definite dal Domain/Application; nessuna dipendenza Supabase nei casi d’uso.
- Configurare URL e chiavi con secret manager o variabili d’ambiente; chiavi service role solo server-side e mai nel client.
- Modellare tabelle/relazioni, vincoli, policy di accesso, migrazioni e indici coerenti con i casi d’uso.
- Gestire cancellazione/timeout, retry limitati e idempotenza; distinguere errore tecnico da rifiuto di business.
- Stabilire se il motore usa query live o snapshot versionati; definire comportamento quando Supabase è indisponibile.
- Tenere integrazione e test locali riproducibili; raccogliere log strutturati e metriche senza esporre segreti o dati personali.

**Esempio:** `IHubRepository` ha adapter JSON per esercizi e adapter Supabase per ambiente condiviso; la pianificazione riceve un `NetworkSnapshot` con numero di versione, evitando query per ogni arco durante BFS/Dijkstra.

## Risultato ADVANCED

Un gestionale e un motore logistico separati a livello di moduli, con casi d’uso verificabili, API/Console come presentation layer, test d’integrazione riproducibili, algoritmi scelti e misurati, persistenza modellata sulle query e integrazione Supabase configurabile e resiliente.
