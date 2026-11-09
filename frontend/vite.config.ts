import { defineConfig, loadEnv } from 'vite'
import vue from '@vitejs/plugin-vue'

export default defineConfig(({ mode }) => {
  // Lo stesso GOOGLE_CLIENT_ID della root serve a Compose e al frontend avviato con `npm run dev`.
  const rootEnv = loadEnv(mode, '..', '')
  const googleClientId = rootEnv.VITE_GOOGLE_CLIENT_ID || rootEnv.GOOGLE_CLIENT_ID || ''

  return {
    define: { 'import.meta.env.VITE_GOOGLE_CLIENT_ID': JSON.stringify(googleClientId) },
    plugins: [vue()],
    server: { host: 'localhost', port: 5173, strictPort: true },
  }
})
