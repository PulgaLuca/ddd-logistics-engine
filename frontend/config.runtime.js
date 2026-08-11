// Configurazione runtime della SPA.
// Fuori da Docker questo file resta vuoto e config.js usa il fallback locale
// (https://localhost:7086). Nel container nginx viene rigenerato all'avvio
// dallo script docker-entrypoint.d/10-api-base.sh con il valore di API_BASE_URL.
