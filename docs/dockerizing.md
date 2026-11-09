# Dockerizzare LogisticsEngine

Questa guida introduce Docker e descrive i file Docker aggiunti alla repository. Il Compose locale avvia due servizi:

```text
Browser -> frontend (Nginx + Vue) -> /api e /swagger -> api (ASP.NET Core)
                                                       -> MongoDB Atlas
```

Il frontend e l'API sono container separati. Nginx inoltra le richieste `/api` e `/swagger` al servizio `api` usando la rete privata Compose. Nel browser frontend e API hanno così la stessa origine web; non serve aggiungere la porta del frontend Docker alla policy CORS. Il backend espone anche la porta host 5003 per Swagger e per chiamate dirette.

## Che cos'è Docker

Docker permette di confezionare un'applicazione e il suo ambiente in immagini e container. Un'immagine è il pacchetto immutabile da cui avviare l'app; un container è una sua esecuzione isolata con filesystem, rete e processo propri. Più persone possono così usare la stessa versione di runtime e dipendenze senza installarli direttamente sulla macchina.

Il container non è una macchina virtuale completa: condivide il kernel del sistema ospite, ma isola i processi dell'applicazione. I container sono sostituibili; i dati da conservare vanno messi in volumi o sistemi esterni, come Atlas.

## Immagine e container

```text
Dockerfile -- docker build --> Image -- docker run --> Container
compose.yaml -- docker compose up --> servizi + rete + volumi
```

Le immagini sono formate da layer. Docker può riutilizzare i layer invariati per velocizzare le build. I file Dockerfile di questa repository usano build multi-stage: il frontend viene compilato in un'immagine Node e servito da Nginx; il backend viene compilato con l'SDK .NET e avviato con il solo runtime ASP.NET. Gli SDK e le dipendenze di sviluppo non entrano nelle rispettive immagini finali. [La documentazione Docker descrive il vantaggio delle build multi-stage](https://docs.docker.com/get-started/docker-concepts/building-images/multi-stage-builds/).

## Cos'è un Dockerfile

Un `Dockerfile` è una sequenza dichiarativa di istruzioni per costruire un'immagine. I file presenti sono:

- `backend/Dockerfile`: ripristina i pacchetti .NET, pubblica `LogisticsEngine.Api` e copia l'app pubblicata nell'immagine runtime.
- `frontend/Dockerfile`: installa le dipendenze con `npm ci`, compila Vue/Vite e copia gli asset statici nell'immagine Nginx.
- `frontend/nginx.conf`: configura il web server statico e il proxy reverse verso il backend.
- `.dockerignore` e `frontend/.dockerignore`: impediscono di inviare al builder cartelle generate, dipendenze locali, file `.env` e dati locali.

Istruzioni comuni:

| Istruzione | Significato |
| --- | --- |
| `FROM` | Sceglie un'immagine di partenza; ogni `FROM` può iniziare uno stage |
| `WORKDIR` | Imposta la directory di lavoro delle istruzioni successive |
| `COPY` | Copia file dal contesto di build all'immagine |
| `RUN` | Esegue un comando durante la costruzione dell'immagine |
| `ARG` | Definisce una variabile disponibile in fase di build |
| `ENV` | Imposta una variabile disponibile nel container |
| `EXPOSE` | Documenta la porta usata dal processo; non pubblica la porta sull'host |
| `ENTRYPOINT` / `CMD` | Definisce il programma avviato quando parte il container |

Il **contesto di build** è l'insieme di file che Docker può usare come input. Compose usa la root della repository per il backend e `frontend/` per il frontend. I rispettivi `.dockerignore` tengono il contesto piccolo e proteggono i file locali.

## Cos'è Docker Compose

Docker Compose descrive e gestisce più servizi collegati usando un file YAML, qui `compose.yaml`. In questo caso dichiara:

- `api`, costruito dal Dockerfile backend, in ascolto sulla porta 8080 nel container e pubblicato come porta 5003 sulla macchina;
- `frontend`, costruito dal Dockerfile frontend, servito sulla porta 80 nel container e pubblicato come porta 8080 sulla macchina;
- una rete privata predefinita in cui Nginx trova il backend con il nome DNS `api`;
- il volume nominato `logistics-data`, montato in `/app/data` per la persistenza JSON.

MongoDB non viene creato da Compose: questo progetto usa MongoDB Atlas esterno quando `STORAGE_PROVIDER=MongoDb`. Con `STORAGE_PROVIDER=Json`, gli archivi locali vivono nel volume Docker.

Compose è il modo raccomandato per avviare insieme i servizi, mantenendo porte, variabili e volumi in una configurazione ripetibile. [Docker Compose definisce ed esegue applicazioni multi-container](https://docs.docker.com/compose/).

## YAML in breve

YAML è un formato testuale usato per configurazioni strutturate. In Compose:

- le coppie `nome: valore` sono mappe;
- le righe che iniziano con `-` sono elementi di una lista;
- l'indentazione definisce la gerarchia: usare spazi, non tabulazioni;
- virgolette e parentesi non sono generalmente necessarie per valori semplici, ma aiutano per porte e stringhe con caratteri speciali.

Esempio ridotto:

```yaml
services:                       # mappa dei servizi dell'applicazione
  frontend:                     # nome DNS del servizio sulla rete Compose
    build:                      # configurazione dell'immagine da costruire
      context: frontend         # cartella inviata al builder
      dockerfile: Dockerfile    # file con le istruzioni d'immagine
    ports:                      # lista delle porte pubblicate
      - "8080:80"                # host:container
    depends_on:                 # dipendenze di avvio
      - api
```

Nella configurazione reale il frontend passa `VITE_API_BASE_URL=/api/v1` in build; Nginx inoltra quel percorso al servizio `api`. Il servizio backend riceve variabili ambiente per provider, database e chiave di firma token. I dettagli Compose sono nella [specifica ufficiale dei servizi](https://docs.docker.com/reference/compose-file/services/).

## Preparazione delle variabili locali

Serve Docker Desktop con il motore Linux avviato, perché le immagini usano Linux. Apri il Prompt dei comandi nella cartella principale del repository:

```cmd
cd C:\Users\lucap\source\repos\LogisticsEngine
```

Crea `.env` dalla configurazione di esempio, solo se non esiste già:

```cmd
copy .env.example .env
```

La root `.env` configura Compose: porte, provider e signing key. Imposta una signing key casuale di almeno 32 byte prima di usare endpoint di autenticazione. Non committare `.env`.

Per usare MongoDB Atlas, crea `backend/.env` partendo dall'esempio se manca:

```cmd
copy backend\.env.example backend\.env
notepad backend\.env
```

Inserisci URI Atlas, password e database nel file `backend/.env`; sostituisci i segnaposto con i valori reali del cluster e controlla l'allow-list di rete Atlas. Questo file è ignorato da Git e da Docker build. Compose lo passa solo al container API. Non incollare credenziali reali nella documentazione o nel repository.

Per avviare con persistenza JSON locale invece di Atlas, imposta `STORAGE_PROVIDER=Json` nella root `.env`. Il volume `logistics-data` mantiene quei JSON anche dopo la ricreazione dei container.

## Avvio dal Prompt dei comandi

Dalla root del repository:

```cmd
docker compose config --quiet
docker compose up --build -d
docker compose ps
```

`config --quiet` controlla la sintassi e la risoluzione della configurazione senza avviare i servizi. `up --build -d` costruisce le immagini mancanti o modificate e lascia i container in esecuzione in background.

Apri nel browser:

- Frontend: [http://localhost:8080](http://localhost:8080)
- Swagger attraverso Nginx: [http://localhost:8080/swagger](http://localhost:8080/swagger)
- Swagger direttamente dall'API: [http://localhost:5003/swagger](http://localhost:5003/swagger)
- Health check API: [http://localhost:8080/health](http://localhost:8080/health)

Il proxy Nginx instrada `/api/v1/...` al backend; il browser non deve conoscere il nome interno `api` e non usa il certificato HTTPS di sviluppo. Lo stesso hostname Compose `api` funziona soltanto all'interno della rete Docker, non nel browser dell'host.

### Comandi fondamentali

```cmd
docker version
docker compose version
docker compose ps
docker compose logs -f
docker compose logs -f api
docker compose logs -f frontend
docker compose restart api
docker compose stop
docker compose start
docker compose down
docker compose down --volumes
```

- `logs -f` segue i log; `Ctrl+C` interrompe la visualizzazione ma lascia i container in esecuzione.
- `stop` ferma i container senza rimuoverli; `start` li riavvia.
- `down` rimuove container e rete Compose, ma conserva il volume dei dati.
- `down --volumes` rimuove anche i volumi e cancella i JSON salvati in `logistics-data`: usalo solo se vuoi davvero eliminare quei dati. La distinzione tra rimozione container e volumi è descritta nella [guida ufficiale Compose](https://docs.docker.com/compose/gettingstarted/).

Comandi Docker di base, indipendenti da Compose:

```cmd
docker images
docker ps -a
docker pull nginx:alpine
docker build -t logisticsengine-frontend frontend
docker run --rm -p 8080:80 logisticsengine-frontend
docker stop NOME_O_ID_CONTAINER
docker rm NOME_O_ID_CONTAINER
```

`docker run` avvia una singola immagine; per questa applicazione è preferibile `docker compose up`, perché la UI dipende dall'API e dalla rete configurata.

## Gestione da Docker Desktop (GUI)

1. Avvia Docker Desktop e attendi che indichi che il motore è in esecuzione.
2. Esegui una prima volta `docker compose up --build -d` dal Prompt dei comandi nella root. Compose prepara e avvia il gruppo di servizi dalla configurazione YAML.
3. In Docker Desktop apri **Containers** e cerca il progetto `logisticsengine` (o il nome mostrato per questa cartella). Espandi il progetto per vedere `api` e `frontend`.
4. Usa le azioni del progetto per avviare, fermare o riavviare il gruppo; seleziona un singolo container per consultare **Logs**, dettagli, porte e statistiche. La porta 8080 del frontend può essere aperta nel browser dalla vista container.
5. Per rimuovere lo stack, usa `docker compose down` nel terminale. Se elimini il progetto dalla GUI, verifica cosa viene rimosso: il volume nominato è la persistenza dei file JSON.

La GUI è utile per osservare e gestire i container già creati; `compose.yaml` resta la configurazione versionata e riproducibile. La [vista Containers di Docker Desktop](https://docs.docker.com/desktop/use-desktop/container/) mostra anche applicazioni Compose raggruppate e permette di visualizzarne i log.

## Modifiche e ricostruzione

Dopo una modifica a sorgenti o Dockerfile:

```cmd
docker compose up --build -d
```

Solo per forzare una nuova build senza avviare subito i servizi:

```cmd
docker compose build --no-cache
docker compose up -d
```

Se cambi `VITE_API_BASE_URL`, serve ricostruire l'immagine frontend: le variabili `VITE_` sono incorporate negli asset durante `npm run build`.

## Risoluzione problemi

- **Docker Engine non disponibile:** apri Docker Desktop e attendi lo stato “Running”, poi ripeti i comandi. `docker compose config --quiet` può funzionare anche quando il motore non è avviato; build e avvio invece richiedono il daemon.
- **La UI non raggiunge l'API:** verifica che entrambi i servizi siano attivi con `docker compose ps`, poi leggi `docker compose logs api frontend`. In Compose il browser chiama il proxy `/api`; il proxy deve raggiungere il servizio `api` sulla rete Compose.
- **Swagger non apre:** controlla che `ASPNETCORE_ENVIRONMENT` sia `Development` nel servizio API e che `/swagger` sia inoltrato da Nginx.
- **Errore Atlas o timeout:** rivedi URI/password nel file `backend/.env`, credenziali del database e allow-list IP/network access di Atlas.
- **Porta occupata:** modifica `API_PORT` o `FRONTEND_PORT` nella root `.env`, poi ricrea i servizi con `docker compose up -d`.
- **Dati JSON scomparsi:** controlla che il volume Compose sia montato e che `STORAGE_PROVIDER=Json`. Non usare `down --volumes` se vuoi conservare i dati.

## Nota per l'ambiente reale

Questo Compose è pensato per apprendimento e sviluppo: espone Swagger, usa l'ambiente ASP.NET `Development` e legge variabili locali. Per un deployment reale occorrono segreti gestiti in modo sicuro, HTTPS terminato correttamente davanti all'API, una configurazione ASP.NET non Development, limiti di rete e policy di autorizzazione applicate anche agli endpoint Hub e Shipment.
