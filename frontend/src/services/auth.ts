import { request } from './httpClient'

export interface AuthUser { id: string; userName: string; role: string; displayName?: string | null; firstName?: string; lastName?: string; professionalRole?: string }
export interface AuthSession { accessToken: string; tokenType: string; expiresAtUtc: string; user: AuthUser }
const sessionKey = 'logisticsengine.auth'

export function getSession(): AuthSession | null {
  try {
    const value = localStorage.getItem(sessionKey)
    if (!value) return null
    const session = JSON.parse(value) as AuthSession
    if (Date.parse(session.expiresAtUtc) <= Date.now()) { localStorage.removeItem(sessionKey); return null }
    return session
  } catch { localStorage.removeItem(sessionKey); return null }
}

export async function signInWithGoogle(credential: string): Promise<AuthSession> {
  const session = await request<AuthSession>('/users/google', { method: 'POST', body: JSON.stringify({ credential }) })
  localStorage.setItem(sessionKey, JSON.stringify(session))
  window.dispatchEvent(new Event('auth-changed'))
  return session
}

export function signOut(): void {
  localStorage.removeItem(sessionKey)
  window.dispatchEvent(new Event('auth-changed'))
}

export function updateSessionUser(user: AuthUser): void {
  const session = getSession()
  if (!session) return
  const updated = { ...session, user }
  localStorage.setItem(sessionKey, JSON.stringify(updated))
  window.dispatchEvent(new Event('auth-changed'))
}
