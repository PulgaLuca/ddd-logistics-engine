# Persistenza MongoDB Atlas

Questa guida introduce MongoDB e mostra come il layer Infrastructure di LogisticsEngine può usare Atlas per salvare i documenti. La persistenza JSON resta disponibile per le prove locali senza connessione Atlas.

## Che cos'è MongoDB

MongoDB è un database NoSQL orientato ai documenti. Invece di organizzare i dati principalmente in righe e colonne relazionate da tabelle, conserva documenti composti da coppie campo-valore. I documenti simili sono raggruppati in collezioni.

MongoDB non impone una tabella relazionale con tutte le colonne dichiarate prima dell'inserimento. Questo schema flessibile non elimina la necessità di progettare i dati: l'applicazione deve mantenere invarianti, validare ciò che legge e introdurre migrazioni o compatibilità quando cambia la forma dei documenti.

## Atlas, cluster, database e collezioni

MongoDB Atlas è il servizio gestito cloud per ospitare cluster MongoDB. Atlas si occupa di provisioning e gestione del cluster; l'applicazione si connette con il driver .NET usando una URI. Per stabilire la connessione servono una URI valida, un database user autorizzato e l'indirizzo client ammesso nella IP access list del progetto Atlas.

La gerarchia usata nel progetto è:

```text
Atlas project
└── Cluster
    └── Database: logisticsengine
        ├── hubs
        ├── hubConnections
        ├── shipments
        └── users
```

Una collezione svolge un ruolo simile a un insieme di record di una tabella, ma contiene documenti e non richiede che ogni documento abbia esattamente gli stessi campi. MongoDB crea database e collezioni quando viene scritto il primo documento, salvo che siano creati esplicitamente prima.

## BSON: il formato memorizzato

BSON significa **Binary JSON**. È il formato binario che MongoDB usa per rappresentare i documenti. È ispirato alla notazione JSON, ma distingue più tipi di dato e si presta alla serializzazione binaria. JSON è comodo da leggere e scambiare in API; BSON è il formato memorizzato e trasmesso dal driver.

Un documento Hub mostrato in forma JSON leggibile può essere:

```json
{
  "_id": "b6c9c96e-8525-4c70-a4cd-d799b2b3e3a1",
  "name": "Milano",
  "zone": "NORD"
}
```

Nel documento BSON, `_id` è il campo identificativo della collezione. Se non viene fornito, MongoDB può generarne uno; qui l'applicazione usa i GUID dell'entità come ID e specifica la rappresentazione standard UUID subtype 4 per interoperabilità fra driver. I collegamenti Hub hanno invece un ID composto deterministico. La distanza di una connessione è salvata come tipo numerico compatibile con decimal (Decimal128 nel driver). Gli enum Shipment e UserRole sono mappati come valori enum dal driver.

Il driver ufficiale C# converte POCO/document class in BSON e viceversa. I repository Mongo traducono i documenti in entità e value object con i metodi `Create` del Domain: i dati letti non vengono considerati validi solo perché provengono dal database.

## Dove sono implementati i repository

Le interfacce `IHubRepository`, `IHubConnectionRepository`, `IShipmentRepository` e `IUserRepository` restano nel Domain. Nel progetto Infrastructure esistono le due famiglie di implementazioni:

| Contratto | File JSON | MongoDB |
|---|---|---|
| Hub | `Infrastructure/Hubs/JsonHubRepository.cs` | `Infrastructure/Hubs/MongoHubRepository.cs` |
| Collegamenti Hub | `Infrastructure/Hubs/JsonHubConnectionRepository.cs` | `Infrastructure/Hubs/MongoHubConnectionRepository.cs` |
| Shipment | `Infrastructure/Shipments/JsonShipmentRepository.cs` | `Infrastructure/Shipments/MongoShipmentRepository.cs` |
| User | `Infrastructure/Users/JsonUserRepository.cs` | `Infrastructure/Users/MongoUserRepository.cs` |

`AddInfrastructure` registra una sola famiglia alla volta. I repository usano un unico `MongoClient` singleton: il driver gestisce e riutilizza il proprio pool di connessioni. L'API imposta Stable API v1 nelle opzioni del client.

La collezione `users` conserva `passwordHash`, mai la password in chiaro. Il repository crea anche un indice univoco sul nome utente normalizzato. I collegamenti Hub hanno un `_id` deterministico composto dai due ID ordinati, così lo stesso arco non diretto non viene inserito due volte.

Il repository Hub legge sia `_id` BSON UUID standard (usato dai nuovi record dell'applicazione) sia documenti legacy con campi aggiuntivi come `Id`, `Name` e `Zone` maiuscoli, comuni quando i dati vengono importati da JSON/Compass. Se il campo legacy `Id` contiene un GUID valido, lo mantiene come identità di dominio; altrimenti ricava un GUID stabile dall'`ObjectId`. Le query accettano entrambe le forme e gli aggiornamenti normalizzano i nomi dei campi senza tentare di cambiare il campo `_id` immutabile.

## Credenziali in `.env`

Il file `backend/.env` è escluso da Git. Il file [backend/.env.example](../backend/.env.example) contiene lo schema da copiare e completare. In questa richiesta sono arrivati i nomi `MONGODB_PASSWORD` e `MONGODB_URI`, ma non i valori: il `.env` locale è quindi predisposto con variabili vuote e non contiene credenziali inventate.

Apri **Database > Connect > Drivers** in Atlas, copia la URI per C#/.NET e inseriscila in `MONGODB_URI`. La forma di esempio è:

```dotenv
MONGODB_URI=mongodb+srv://<db_username>:<db_password>@<cluster-host>/?retryWrites=true&w=majority&appName=LogisticsEngine
MONGODB_PASSWORD=la-password-del-database-user
MONGODB_DATABASE=logisticsengine
```

Nel tuo `.env`, sostituisci `<db_username>` e `<cluster-host>` con i valori forniti da Atlas. Codifica anche eventuali caratteri riservati nel nome utente. Il loader sostituisce `<db_password>` usando `MONGODB_PASSWORD` e codifica i caratteri riservati per l'uso nella URI. In alternativa, inserisci una URI già completa senza segnaposto; in quel caso il valore separato della password non viene usato.

Il loader cerca `.env` partendo dalla directory di esecuzione e dai parent, legge le variabili prima di costruire la configurazione ASP.NET Core e non sovrascrive variabili d'ambiente già impostate. Non registrare `.env` in Git, non stamparne il contenuto e non inserire la URI in log o screenshot. In ambienti distribuiti, usa un secret manager o variabili protette del servizio invece di copiare il file.

### Configurazione del provider

- Se `MONGODB_URI` è valorizzata e `STORAGE_PROVIDER` non è impostato, l'API seleziona MongoDB.
- `STORAGE_PROVIDER=MongoDb` seleziona esplicitamente Atlas e richiede una URI valida.
- `STORAGE_PROVIDER=Json` forza la persistenza JSON anche se una URI è presente.
- Senza URI e senza selezione esplicita, l'API continua a usare JSON.

Il database predefinito è `logisticsengine`; `MONGODB_DATABASE` permette di cambiarlo per ambienti separati. La console continua a usare le implementazioni JSON.

## Avvio e prova

1. Crea in Atlas un database user con i permessi minimi necessari per leggere, scrivere e creare indici nel database `logisticsengine`.
2. Aggiungi l'indirizzo IP della macchina alla IP access list Atlas.
3. Copia `backend/.env.example` in `backend/.env` e inserisci URI e password reali.
4. Dalla cartella `backend`, avvia l'API:

   ```powershell
   dotnet run --project src/LogisticsEngine.Api --launch-profile http
   ```

5. Crea una risorsa da Swagger UI su `http://localhost:5003/swagger`, per esempio un Hub o uno Shipment. La prima scrittura crea il database e la relativa collezione, se non esistono.
6. Verifica la risorsa con la richiesta GET corrispondente o nella vista Browse Collections di Atlas.

Per usare JSON, imposta `STORAGE_PROVIDER=Json` nel `.env` oppure rimuovi temporaneamente `MONGODB_URI`.

## Cosa succede durante una richiesta

Per esempio, `POST /api/v1/hubs` attraversa i controller e MediatR fino al caso d'uso. L'handler riceve `IHubRepository`; il container DI ha registrato `MongoHubRepository` quando il provider è MongoDB. Il repository converte l'Hub in un documento BSON e lo inserisce nella collezione `hubs`. Una successiva GET esegue una query, materializza il documento e ricostruisce l'entità attraverso le regole del Domain.

In questo modo il caso d'uso non sa se i dati arrivano da un file o da Atlas: conosce solo il contratto del repository.

## Configurazione e problemi frequenti

- **Authentication failed:** verifica database username e password dell'Atlas Database Access, che sono distinti dall'account usato per accedere al sito Atlas.
- **IP non autorizzato:** aggiungi l'IP client alla IP access list; non usare `0.0.0.0/0` come impostazione permanente.
- **URI parse/authentication error:** caratteri come `@`, `:` e `/` nella password devono essere percent-encoded se sono inclusi direttamente nella URI. Con il segnaposto `<db_password>`, se ne occupa il loader.
- **Connessione DNS SRV:** Atlas usa tipicamente `mongodb+srv://`; la rete deve consentire la risoluzione DNS SRV e il traffico in uscita richiesto dal cluster.
- **Le vecchie risorse JSON non appaiono:** JSON e MongoDB sono provider separati; i file non vengono migrati o sincronizzati automaticamente.
- **Utenti già presenti senza indice univoco:** il primo accesso/registrazione Mongo tenta di creare l'indice univoco; eventuali duplicati esistenti vanno risolti prima.

## Riferimenti

- [MongoDB .NET/C# Driver: collegarsi a un deployment](https://www.mongodb.com/docs/drivers/csharp/current/connect/connection-targets/)
- [MongoDB .NET/C# Driver: database e collezioni](https://www.mongodb.com/docs/drivers/csharp/current/databases-collections/)
- [MongoDB .NET/C# Driver: formato BSON](https://www.mongodb.com/docs/drivers/csharp/current/document-formats/bson/)
- [MongoDB .NET/C# Driver: rappresentazione dei GUID](https://www.mongodb.com/docs/drivers/csharp/current/serialization/guids/)
- [Atlas: configurare la IP access list](https://www.mongodb.com/docs/atlas/security/add-ip-address-to-list/)
- [Atlas: password speciali nelle URI](https://www.mongodb.com/docs/atlas/troubleshoot-connection/)
