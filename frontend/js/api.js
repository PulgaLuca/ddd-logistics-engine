// Client HTTP (AJAX via fetch) verso l'API .NET.
import { getApiBase } from './config.js';
import { auth, clearAuth } from './store.js';

export class ApiError extends Error {
  constructor(status, problem) {
    super(problem?.detail || problem?.title || `HTTP ${status}`);
    this.status = status;
    this.problem = problem;
  }
}

async function request(method, path, body) {
  const headers = { Accept: 'application/json' };

  if (auth.user?.accessToken) {
    headers.Authorization = `Bearer ${auth.user.accessToken}`;
  }

  const options = { method, headers };
  if (body !== undefined) {
    headers['Content-Type'] = 'application/json';
    options.body = JSON.stringify(body);
  }

  let response;
  try {
    response = await fetch(`${getApiBase()}${path}`, options);
  } catch {
    throw new ApiError(0, {
      title: 'Impossibile contattare l\'API',
      detail: `Nessuna risposta da ${getApiBase()}. Verifica che l'API sia avviata e che il certificato HTTPS sia attendibile.`
    });
  }

  if (response.status === 401 && auth.user?.accessToken) {
    clearAuth();
  }

  if (!response.ok) {
    let problem = null;
    try {
      problem = await response.json();
    } catch {
      /* corpo non JSON */
    }
    throw new ApiError(response.status, problem);
  }

  if (response.status === 204) {
    return null;
  }

  const text = await response.text();
  return text ? JSON.parse(text) : null;
}

export const api = {
  get: (path) => request('GET', path),
  post: (path, body) => request('POST', path, body)
};

export function errorMessage(error) {
  if (error instanceof ApiError && error.problem) {
    const details = error.problem.errors?.map((e) => e.description || e.code).join(' · ');
    const base = error.problem.detail || error.problem.title || `HTTP ${error.status}`;
    return details ? `${base} (${details})` : base;
  }
  return error?.message || 'Errore imprevisto';
}
