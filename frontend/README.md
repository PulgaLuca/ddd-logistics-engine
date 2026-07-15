# Logistics Engine — Frontend (Vue 3)

Single Page Application in **Vue 3** (via CDN + import map, **senza build step**), che consuma
l'API .NET via **AJAX (fetch)**. Include una **landing page**, login/registrazione, un'area
gestionale post-login e **visualizzazioni grafiche** di grafi, alberi, code e stack tramite
[vis-network](https://visjs.github.io/vis-network/).

## Struttura

```
frontend/
├─ index.html            # shell: import map (vue, vue-router), Bootstrap, vis-network
├─ css/styles.css        # tema minimale (landing, sidebar, grafici)
└─ js/
   ├─ config.js          # URL base dell'API (persistito nel browser)
   ├─ store.js           # stato auth reattivo + helper permessi
   ├─ api.js             # client fetch (Bearer token, ProblemDetails)
   ├─ toast.js           # notifiche
   ├─ format.js          # etichette/valori leggibili (niente JSON grezzo)
   ├─ router.js          # rotte + guardia di autenticazione
   ├─ root.js            # componente radice (router-view + toast)
   ├─ app.js             # createApp + registrazione componenti globali
   ├─ components/
   │  ├─ DataView.js      # mostra un oggetto/lista come CAMPI (ricorsivo)
   │  ├─ GraphPanel.js    # wrapper vis-network (grafi e alberi)
   │  └─ QueueViz.js      # coda come sequenza di carte
   └─ views/
      ├─ Landing.js  ├─ Login.js  ├─ Register.js
      ├─ AppShell.js (layout con sidebar)  ├─ Dashboard.js
      ├─ Shipments.js  ├─ Orders.js  ├─ Dispatch.js
      ├─ Hubs.js  ├─ Zones.js (albero)  └─ Network.js (grafo + percorsi)
```

## Visualizzazioni degli algoritmi

- **Rete & Percorsi** (`Network.js`): la rete stradale come **grafo** (hub = nodi, strade = archi
  etichettati con distanza e traffico). Dijkstra, A* e la raggiungibilità **BFS/DFS** evidenziano
  il percorso trovato direttamente sul grafo, con l'ordine di visita.
- **Zone** (`Zones.js`): l'**albero** delle zone (Zona → Sottozona → Vie) reso in layout gerarchico.
- **Ordini** (`Orders.js`): le **code** FIFO e a priorità come sequenza visiva di carte.
- **Dispatch** (`Dispatch.js`): le pile **undo/redo** (Stack) mostrate come colonne impilate.

## Come avviarlo

### 1. Avvia l'API .NET
```
cd backend/src/Dhexby.LogisticsEngine.Api
dotnet run --launch-profile https        # https://localhost:7086
dotnet dev-certs https --trust            # la prima volta
```
Per provare senza MongoDB: `"Persistence": { "Provider": "Json" }` in `appsettings.json`.

### 2. Servi il frontend
Vue e i moduli richiedono un server HTTP (non aprire `index.html` con `file://`).
Da dentro `frontend/`:
```
python -m http.server 8080     # oppure:  npx serve .
```
Apri l'URL mostrato (es. http://localhost:8080).

### 3. Usa l'app
1. Dalla landing scegli **Registrati** o **Accedi** (in Login, sotto "Impostazioni avanzate",
   puoi cambiare l'URL dell'API se non è `https://localhost:7086`).
2. Dopo il login entri nell'area gestionale (sidebar): le azioni non consentite dal tuo ruolo
   appaiono **bloccate** e vengono comunque respinte dall'API (403).

## Ruoli

| Ruolo | Può fare |
|---|---|
| **Administrator** | Tutto |
| **Manager** | Creare/gestire dati + azioni operative + lettura |
| **Operator** | Azioni operative (dispatch, assign, undo/redo, vie) + lettura |
| **Viewer** | Sola lettura e visualizzazioni |

> La CORS lato API è abilitata in sviluppo (`Program.cs`). Autenticazione con token Bearer (JWT).
