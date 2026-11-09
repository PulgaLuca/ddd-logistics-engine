<script setup lang="ts">
import { onMounted, onUnmounted, ref } from 'vue'
import { RouterLink, RouterView } from 'vue-router'
import { getSession, signInWithGoogle, signOut, type AuthSession } from './services/auth'

declare global { interface Window { google?: { accounts: { id: { initialize(options: { client_id: string; callback: (response: { credential: string }) => void }): void; renderButton(element: HTMLElement, options: Record<string, string>): void; prompt(): void } } } } }

const session = ref<AuthSession | null>(getSession())
const googleButtonSlot = ref<HTMLElement | null>(null)
const authError = ref('')
const googleClientId = import.meta.env.VITE_GOOGLE_CLIENT_ID
const googleReady = ref(false)
const officialGoogleButtonRendered = ref(false)
const refreshSession = () => { session.value = getSession() }
const onGoogleCredential = async (response: { credential: string }) => {
  try { session.value = await signInWithGoogle(response.credential); authError.value = '' }
  catch (error) { authError.value = error instanceof Error ? error.message : 'Accesso Google non riuscito.' }
}
function initializeGoogle() {
  if (!window.google || !googleClientId || !googleButtonSlot.value) return
  window.google.accounts.id.initialize({ client_id: googleClientId, callback: onGoogleCredential })
  googleReady.value = true
  try {
    window.google.accounts.id.renderButton(googleButtonSlot.value, { type: 'standard', theme: 'outline', size: 'medium', text: 'continue_with', shape: 'rectangular', locale: 'it' })
    officialGoogleButtonRendered.value = true
  } catch { officialGoogleButtonRendered.value = false }
}
function beginGoogleSignIn() {
  if (!googleClientId) { authError.value = 'Configura VITE_GOOGLE_CLIENT_ID per abilitare l’accesso Google.'; return }
  if (!googleReady.value || !window.google) { authError.value = 'Il servizio Google non è ancora disponibile. Riprova tra qualche secondo.'; return }
  authError.value = ''
  window.google.accounts.id.prompt()
}
onMounted(() => {
  window.addEventListener('auth-changed', refreshSession)
  const existing = document.querySelector<HTMLScriptElement>('script[data-google-identity]')
  if (existing) { existing.addEventListener('load', initializeGoogle); initializeGoogle(); return }
  const script = document.createElement('script')
  script.src = 'https://accounts.google.com/gsi/client'; script.async = true; script.defer = true; script.dataset.googleIdentity = 'true'
  script.onload = initializeGoogle; document.head.appendChild(script)
})
onUnmounted(() => window.removeEventListener('auth-changed', refreshSession))

const navigation = [
  { to: '/overview', label: 'Panoramica', icon: '◫' },
  { to: '/hubs', label: 'Hub', icon: '⌖' },
  { to: '/shipments', label: 'Spedizioni', icon: '↗' },
  { to: '/network', label: 'Rete & algoritmi', icon: '⌘' },
  { to: '/profile', label: 'Il mio profilo', icon: '◉' },
]
</script>

<template>
  <div class="app-frame">
    <aside class="sidebar">
      <RouterLink class="brand" to="/overview"><span class="brand-mark">L</span><span>logistics<span class="brand-light">engine</span></span></RouterLink>
      <p class="nav-caption">WORKSPACE</p>
      <nav class="side-nav" aria-label="Navigazione principale">
        <RouterLink v-for="item in navigation" v-show="item.to !== '/profile' || session" :key="item.to" :to="item.to" class="nav-link"><span class="nav-icon">{{ item.icon }}</span>{{ item.label }}</RouterLink>
      </nav>
      <div class="sidebar-bottom"><span class="status-dot"></span><div><strong>API v1</strong><small>Ambiente di sviluppo</small></div><span class="api-pill">DEV</span></div>
    </aside>
    <div class="main-column">
      <header class="topbar"><div><span class="breadcrumb">LogisticsEngine</span><span class="breadcrumb-separator">/</span><span class="breadcrumb current">Workspace</span></div><div class="topbar-actions"><a class="api-link" href="https://localhost:7259/swagger" target="_blank" rel="noreferrer">Apri Swagger <span>↗</span></a><div v-show="!session" class="google-auth-wrap"><div ref="googleButtonSlot" class="google-button-slot" :class="{ 'google-button-slot-ready': officialGoogleButtonRendered }"></div><button v-if="!officialGoogleButtonRendered" class="google-login-button" @click="beginGoogleSignIn"><svg class="google-mark" viewBox="0 0 48 48" aria-hidden="true"><path fill="#4285F4" d="M43.6 24.5c0-1.4-.1-2.8-.4-4.1H24v7.8h11a9.4 9.4 0 0 1-4.1 6.2v5h6.6c3.9-3.6 6.1-8.8 6.1-14.9Z"/><path fill="#34A853" d="M24 44c5.5 0 10.1-1.8 13.5-4.8l-6.6-5c-1.8 1.2-4.1 2-6.9 2-5.3 0-9.8-3.6-11.4-8.4H5.8v5.2A20 20 0 0 0 24 44Z"/><path fill="#FBBC05" d="M12.6 27.8a12 12 0 0 1 0-7.6V15H5.8a20 20 0 0 0 0 17.2l6.8-4.4Z"/><path fill="#EA4335" d="M24 11.8c3 0 5.7 1 7.8 3.1l5.8-5.8C34.1 5.8 29.5 4 24 4A20 20 0 0 0 5.8 15l6.8 5.2c1.6-4.8 6.1-8.4 11.4-8.4Z"/></svg><span>Registrati / accedi</span></button></div><div v-if="session" class="user-session"><span class="user-avatar">{{ (session.user.displayName || session.user.userName).charAt(0).toUpperCase() }}</span><span class="user-name">{{ session.user.displayName || session.user.userName }}</span><button class="button button-outline logout-button" @click="signOut(); session = null">Logout</button></div></div></header>
      <p v-if="authError" class="auth-error" role="alert">{{ authError }}</p>
      <main class="content"><RouterView /></main>
      <footer class="app-footer">Demo didattica · Vue 3 · API versionata</footer>
    </div>
  </div>
</template>
