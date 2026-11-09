# Autenticazione con Google

Il frontend usa Google Identity Services per ottenere un ID token. L'API lo verifica con la libreria ufficiale Google.Apis.Auth, controllando firma, scadenza e audience (Client ID). Dopo la verifica, crea un utente locale con ruolo `Viewer` al primo accesso e restituisce il normale token bearer LogisticsEngine. L'identità locale è legata al `sub` Google, non all'email.

## 1. Crea un client OAuth

1. Apri [Google Cloud Console](https://console.cloud.google.com/) e seleziona o crea un progetto.
2. Configura la schermata di consenso OAuth, includendo gli utenti di test se l'app è in modalità Testing.
3. In **API e servizi → Credenziali**, crea un **ID client OAuth** di tipo **Applicazione web**.
4. In **Origini JavaScript autorizzate**, aggiungi ogni origine da cui sarà servito il frontend, ad esempio `http://localhost:8080`, `http://localhost:5173` o il dominio HTTPS di produzione. Inserisci solo schema, host e porta, senza path.
5. Non serve un client secret: questo flusso usa Google Identity Services lato browser e verifica l'ID token nell'API.

## 2. Configura il progetto

Copia `.env.example` in `.env` nella root e valorizza `GOOGLE_CLIENT_ID` con l'ID client creato, ad esempio:

```dotenv
GOOGLE_CLIENT_ID=1234567890-example.apps.googleusercontent.com
```

Con Docker Compose, il valore viene passato all'API come `Authentication:Google:ClientId` e incorporato nel frontend come `VITE_GOOGLE_CLIENT_ID`. Ricostruisci il frontend dopo aver cambiato il Client ID, perché le variabili `VITE_*` sono incluse negli asset al build.

Per eseguire Vite e API separatamente, imposta `VITE_GOOGLE_CLIENT_ID` nell'ambiente di build/avvio del frontend e `Authentication__Google__ClientId` nell'ambiente dell'API (oppure `Authentication:Google:ClientId` in `appsettings.*.json`). L'origine del browser deve essere autorizzata nella console Google e ammessa dalla policy CORS dell'API.

## 3. Flusso applicativo

- Il pulsante Google in alto consente registrazione e login con lo stesso flusso.
- Il browser invia il credential ID token a `POST /api/v1/users/google`.
- L'API verifica il token e l'audience; un token non valido o un Client ID non configurato restituisce `401`.
- Al primo accesso viene creato un utente `Viewer`; agli accessi successivi si recupera lo stesso account tramite subject Google.
- Il token bearer applicativo viene salvato nel `localStorage` per la sessione del browser e inviato nelle richieste API. Logout lo rimuove dal browser.
- L'utente autenticato può leggere e aggiornare il proprio profilo con `GET /api/v1/users/me` e `PUT /api/v1/users/me`. Il profilo contiene nome, cognome e ruolo professionale; il ruolo account (`Viewer`, `Operator`, `Administrator`) resta distinto e non è modificabile da questa pagina.

La registrazione e l'autenticazione con username/password già disponibili restano utilizzabili tramite gli endpoint esistenti.
