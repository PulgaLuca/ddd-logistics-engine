import { computed } from 'vue';
import { auth, canManageData, canOperate } from '../store.js';

const SECTIONS = [
  { to: '/app/shipments', icon: 'bi-box-seam', title: 'Spedizioni', text: 'Crea, consulta e spedisci.' },
  { to: '/app/orders', icon: 'bi-receipt', title: 'Ordini', text: 'Registra, assegna e osserva le code.' },
  { to: '/app/dispatch', icon: 'bi-arrow-counterclockwise', title: 'Dispatch', text: 'Undo / redo delle assegnazioni (Stack).' },
  { to: '/app/hubs', icon: 'bi-buildings', title: 'Hub', text: 'I nodi della rete logistica.' },
  { to: '/app/zones', icon: 'bi-diagram-2', title: 'Zone', text: 'Albero delle zone di consegna.' },
  { to: '/app/network', icon: 'bi-share', title: 'Rete & Percorsi', text: 'Grafo, Dijkstra, A*, BFS/DFS.' }
];

export default {
  name: 'Dashboard',
  setup() {
    return {
      auth,
      sections: SECTIONS,
      manage: computed(() => canManageData()),
      operate: computed(() => canOperate())
    };
  },
  template: `
    <div>
      <h1 class="page-title h3 mb-1">Ciao, {{ auth.user?.email }}</h1>
      <p class="section-sub mb-4">Ecco cosa puoi fare con il ruolo <strong>{{ auth.user?.role }}</strong>.</p>

      <div class="row g-3 mb-4">
        <div class="col-md-4">
          <div class="card h-100"><div class="card-body">
            <span class="feature-icon mb-2"><i class="bi bi-eye"></i></span>
            <h2 class="card-title">Lettura</h2>
            <p class="section-sub small mb-2">Consultare dati, code, alberi e grafi.</p>
            <span class="badge text-bg-success">Consentito</span>
          </div></div>
        </div>
        <div class="col-md-4">
          <div class="card h-100"><div class="card-body">
            <span class="feature-icon mb-2"><i class="bi bi-tools"></i></span>
            <h2 class="card-title">Azioni operative</h2>
            <p class="section-sub small mb-2">Dispatch, assegnazioni, undo/redo, vie.</p>
            <span class="badge" :class="operate ? 'text-bg-success' : 'text-bg-secondary'">{{ operate ? 'Consentito' : 'Non consentito' }}</span>
          </div></div>
        </div>
        <div class="col-md-4">
          <div class="card h-100"><div class="card-body">
            <span class="feature-icon mb-2"><i class="bi bi-plus-square"></i></span>
            <h2 class="card-title">Creazione dati</h2>
            <p class="section-sub small mb-2">Spedizioni, ordini, hub, zone, strade.</p>
            <span class="badge" :class="manage ? 'text-bg-success' : 'text-bg-secondary'">{{ manage ? 'Consentito' : 'Non consentito' }}</span>
          </div></div>
        </div>
      </div>

      <h2 class="h5 fw-bold mb-3">Sezioni</h2>
      <div class="row g-3">
        <div v-for="s in sections" :key="s.to" class="col-sm-6 col-lg-4">
          <router-link :to="s.to" class="text-decoration-none text-reset">
            <div class="card h-100"><div class="card-body d-flex align-items-start gap-3">
              <span class="feature-icon"><i class="bi" :class="s.icon"></i></span>
              <div>
                <h3 class="h6 fw-bold mb-1">{{ s.title }}</h3>
                <p class="section-sub small mb-0">{{ s.text }}</p>
              </div>
            </div></div>
          </router-link>
        </div>
      </div>
    </div>
  `
};
