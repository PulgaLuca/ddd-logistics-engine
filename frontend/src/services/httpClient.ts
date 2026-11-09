const apiBaseUrl = (import.meta.env.VITE_API_BASE_URL || 'https://localhost:7259/api/v1').replace(/\/$/, '')

/** Centralizza URL, JSON e messaggi HTTP per evitare duplicazioni nei servizi di feature. */
export async function request<T>(path: string, init: RequestInit = {}): Promise<T> {
  const headers = new Headers(init.headers)
  headers.set('Accept', 'application/json')
  const sessionValue = localStorage.getItem('logisticsengine.auth')
  if (sessionValue) {
    try { headers.set('Authorization', `Bearer ${(JSON.parse(sessionValue) as { accessToken: string }).accessToken}`) } catch { localStorage.removeItem('logisticsengine.auth') }
  }
  if (init.body) headers.set('Content-Type', 'application/json')

  let response: Response
  try {
    response = await fetch(`${apiBaseUrl}${path}`, { ...init, headers })
  } catch {
    throw new Error(`Impossibile raggiungere l'API (${apiBaseUrl}). Verifica che sia avviata.`)
  }

  if (!response.ok) {
    const problem = await response.json().catch(() => null) as { detail?: string; title?: string } | null
    throw new Error(problem?.detail || problem?.title || `La richiesta è fallita (HTTP ${response.status}).`)
  }
  if (response.status === 204) return undefined as T
  return response.json() as Promise<T>
}
