// Gestione dell'URL base dell'API .NET (memorizzato nel browser).
const KEY = 'le.apiBase';
const DEFAULT_BASE = 'https://localhost:7086';

export function getApiBase() {
  return localStorage.getItem(KEY) || DEFAULT_BASE;
}

export function setApiBase(value) {
  const clean = (value || '').trim().replace(/\/+$/, '');
  if (clean) {
    localStorage.setItem(KEY, clean);
  }
}
