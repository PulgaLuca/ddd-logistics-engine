// Gestione dell'URL base dell'API .NET (memorizzato nel browser).
const KEY = 'le.apiBase';

// Sviluppo locale con "dotnet run": profilo https di launchSettings.json.
const FALLBACK_BASE = 'https://localhost:7086';

// In Docker il container nginx genera config.runtime.js all'avvio e valorizza
// window.__API_BASE__: la stessa immagine punta così ad ambienti diversi senza
// essere ricostruita. Fuori da Docker il file non definisce nulla e si usa il
// fallback qui sopra.
function defaultBase() {
  const injected = globalThis.__API_BASE__;
  return typeof injected === 'string' && injected.length > 0 ? injected : FALLBACK_BASE;
}

export function getApiBase() {
  return localStorage.getItem(KEY) || defaultBase();
}

export function setApiBase(value) {
  const clean = (value || '').trim().replace(/\/+$/, '');
  if (clean) {
    localStorage.setItem(KEY, clean);
  }
}
