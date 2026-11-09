# Frontend base con Vue.js

## Obiettivo

Il frontend è la parte dell'applicazione con cui interagisce l'utente nel browser. Questa piccola interfaccia dimostrativa mostra come organizzare il codice client e come inviare richieste HTTP alla Web API per leggere e creare hub e spedizioni.

Il frontend presenta dati e raccoglie input. Le regole applicative e di dominio restano nel backend: il browser non accede direttamente a MongoDB o ai file JSON. Il flusso è:

```text
Browser (Vue) -> HTTP/JSON -> API v1 -> Controller -> MediatR -> Application/Domain -> Infrastructure
```

## Che cos'è Vue.js

Vue.js (Vue) è un framework JavaScript per costruire interfacce utente. Divide la pagina in componenti riutilizzabili e aggiorna il DOM quando cambiano i dati osservati. Vue è adatto a una Single Page Application (SPA): il browser carica l'app una volta e il router cambia la vista senza ricaricare tutto il documento.

In questo progetto usiamo Vue 3 con TypeScript e Vite. Un file `.vue` è un Single File Component: può contenere struttura HTML nel template, logica TypeScript nello script e stili. `ref` rende un valore reattivo; ad esempio, quando `hubs.value` cambia dopo una risposta HTTP, Vue aggiorna automaticamente l'elenco visualizzato.

## Struttura minima

```text
frontend/
├── index.html
├── package.json
├── vite.config.ts
├── .env.example
└── src/
    ├── main.ts                 # avvia Vue, router e stili globali
    ├── App.vue                 # struttura comune e navigazione
    ├── router/index.ts         # associa URL alle viste
    ├── types/api.ts            # forme dei dati JSON scambiati
    ├── services/
    │   ├── httpClient.ts       # richiesta fetch, URL base ed errori HTTP
    │   ├── hubsApi.ts          # endpoint relativi agli hub
    │   └── shipmentsApi.ts     # endpoint relativi alle spedizioni
    ├── views/
    │   ├── HubsView.vue        # interfaccia della sezione Hub
    │   └── ShipmentsView.vue   # interfaccia della sezione Spedizioni
    └── style.css               # presentazione condivisa
```

La separazione mantiene ogni responsabilità in un posto facile da trovare: le viste gestiscono stato e interazione, i servizi conoscono gli endpoint, il client HTTP gestisce i dettagli comuni del trasporto. I DTO TypeScript descrivono il JSON restituito dall'API e non sostituiscono gli oggetti di dominio C#.

## Come avviene una chiamata

All'apertura della vista Hub, `HubsView.vue` esegue `hubsApi.getAll()`. Il servizio delega a `request` in `httpClient.ts`, che compone l'URL, esegue `fetch`, controlla la risposta e converte il JSON. La vista aggiorna il proprio stato reattivo e Vue ridisegna l'elenco.

Per creare un hub, il form invia un `POST` JSON con `name` e `zone` a `/api/v1/hubs`. La Web API valida ed elabora la richiesta come gli altri client. La vista ricarica l'elenco dopo il salvataggio. La sezione Spedizioni mostra lo stesso schema con `/api/v1/shipments` e `trackingNumber`.

Esempio equivalente con la Fetch API:

```ts
const response = await fetch('https://localhost:7259/api/v1/hubs')
if (!response.ok) throw new Error(`HTTP ${response.status}`)
const hubs = await response.json()
```

Il client condiviso del progetto aggiunge gestione degli errori e base URL configurabile, evitando di ripetere questo codice in ogni servizio.

## Avvio locale

Prerequisiti: .NET SDK richiesto dalla soluzione, Node.js e npm.

1. Avvia la Web API da `backend`:

   ```powershell
   dotnet run --project src/LogisticsEngine.Api --launch-profile https
   ```

   Il profilo HTTPS espone `https://localhost:7259` (e anche `http://localhost:5003`). La UI Swagger, in sviluppo, è disponibile a `https://localhost:7259/swagger`.

2. In un altro terminale, dalla cartella `frontend`, installa le dipendenze e avvia Vite:

   ```powershell
   npm install
   npm run dev
   ```

3. Apri `http://localhost:5173`. La navigazione consente di consultare e creare hub e spedizioni.

Vite usa l'origine `http://localhost:5173`; la policy CORS di sviluppo dell'API consente tale origine. CORS è un controllo applicato dal browser: l'API deve dichiarare esplicitamente quali origini web possono inviarle richieste. L'autorizzazione CORS non sostituisce autenticazione o autorizzazione.

## Configurare l'URL dell'API

`frontend/.env.example` documenta la configurazione locale. Per cambiare l'indirizzo, crea `frontend/.env.local` (ignorato da Git) con, ad esempio:

```dotenv
VITE_API_BASE_URL=https://localhost:7259/api/v1
```

Riavvia Vite dopo aver modificato un file `.env`. I valori con prefisso `VITE_` sono incorporati nel codice client e sono visibili nel browser: non inserirvi password, token segreti o credenziali del database.

## Verifica e problemi comuni

- Se appare un errore di connessione, controlla che API e frontend siano entrambi avviati e che la base URL punti al profilo corretto.
- Se il browser segnala CORS, verifica che l'origine aperta sia esattamente `http://localhost:5173` e che l'API sia in ambiente Development. `localhost` e `127.0.0.1` sono origini diverse.
- Se l'API restituisce un errore HTTP, il messaggio viene mostrato sopra la vista; per i dettagli si possono consultare la scheda Network degli strumenti sviluppatore e i log backend.
- Se il browser mostra un errore di connessione, verifica che `VITE_API_BASE_URL` usi lo stesso protocollo e la stessa porta su cui l'API è raggiungibile. Con il profilo HTTPS usa `https://localhost:7259/api/v1` e assicurati che il certificato HTTPS di sviluppo sia considerato attendibile dal browser. Dopo una modifica ai file `.env`, riavvia Vite.

Per controllare che il progetto frontend compili:

```powershell
npm run build
```

Il comando produce file statici in `frontend/dist`; la loro pubblicazione è fuori dallo scopo di questo esempio.
