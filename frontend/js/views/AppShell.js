import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { auth, clearAuth } from '../store.js';
import { getApiBase, setApiBase } from '../config.js';
import { toast } from '../toast.js';

const NAV = [
  { to: '/app', label: 'Dashboard', icon: 'bi-grid-1x2' },
  { to: '/app/shipments', label: 'Spedizioni', icon: 'bi-box-seam' },
  { to: '/app/orders', label: 'Ordini', icon: 'bi-receipt' },
  { to: '/app/dispatch', label: 'Dispatch', icon: 'bi-arrow-counterclockwise' },
  { to: '/app/hubs', label: 'Hub', icon: 'bi-buildings' },
  { to: '/app/zones', label: 'Zone', icon: 'bi-diagram-2' },
  { to: '/app/network', label: 'Rete & Percorsi', icon: 'bi-share' }
];

export default {
  name: 'AppShell',
  setup() {
    const router = useRouter();
    const showSettings = ref(false);
    const apiBase = ref(getApiBase());

    function logout() {
      clearAuth();
      toast('Disconnesso');
      router.push('/');
    }

    function saveApi() {
      setApiBase(apiBase.value);
      apiBase.value = getApiBase();
      showSettings.value = false;
      toast('URL API salvato');
    }

    return { auth, nav: NAV, showSettings, apiBase, logout, saveApi };
  },
  template: `
    <div class="app-shell">
      <aside class="sidebar">
        <router-link to="/app" class="brand text-decoration-none">
          <span class="brand-badge"><i class="bi bi-box-seam"></i></span> Logistics
        </router-link>
        <nav class="nav flex-column">
          <router-link v-for="n in nav" :key="n.to" :to="n.to" class="nav-link">
            <i class="bi" :class="n.icon"></i> {{ n.label }}
          </router-link>
        </nav>
      </aside>

      <div class="app-main">
        <div class="topbar d-flex align-items-center justify-content-between">
          <div class="d-flex align-items-center gap-2">
            <span class="badge text-bg-primary badge-role">{{ auth.user?.role }}</span>
            <span class="section-sub small d-none d-md-inline">{{ auth.user?.email }}</span>
          </div>
          <div class="d-flex align-items-center gap-2">
            <button class="btn btn-sm btn-outline-secondary" @click="showSettings = true" title="Impostazioni">
              <i class="bi bi-gear"></i>
            </button>
            <button class="btn btn-sm btn-outline-danger" @click="logout">Esci</button>
          </div>
        </div>

        <div class="app-content">
          <router-view />
        </div>
      </div>

      <div v-if="showSettings" class="overlay" @click.self="showSettings = false">
        <div class="card">
          <div class="card-body">
            <h2 class="card-title mb-3">Impostazioni</h2>
            <label class="form-label">URL base dell'API .NET</label>
            <input class="form-control" v-model="apiBase" placeholder="https://localhost:7086" />
            <p class="section-sub small mt-2 mb-3">Se usi HTTPS, fidati del certificato: <code>dotnet dev-certs https --trust</code>.</p>
            <div class="d-flex justify-content-end gap-2">
              <button class="btn btn-outline-secondary" @click="showSettings = false">Annulla</button>
              <button class="btn btn-brand" @click="saveApi">Salva</button>
            </div>
          </div>
        </div>
      </div>
    </div>
  `
};
