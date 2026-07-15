import { computed } from 'vue';
import { isLoggedIn } from '../store.js';

const FEATURES = [
  { icon: 'bi-diagram-3', title: 'DDD + Clean Architecture', text: 'Dominio, Application, Infrastructure e API separati, con Result pattern e Value Object.' },
  { icon: 'bi-shield-lock', title: 'Ruoli e sicurezza', text: 'Autenticazione JWT e autorizzazione per ruolo: Administrator, Manager, Operator, Viewer.' },
  { icon: 'bi-collection', title: 'Code e priorità', text: 'Processamento ordini FIFO e coda a priorità per l\'express delivery, visualizzate.' },
  { icon: 'bi-arrow-counterclockwise', title: 'Undo / Redo', text: 'Cronologia delle assegnazioni con struttura a Stack: annulla e ripeti.' },
  { icon: 'bi-diagram-2', title: 'Albero delle zone', text: 'Zone → sottozone → vie, rese come albero interattivo.' },
  { icon: 'bi-share', title: 'Grafo e percorsi minimi', text: 'Rete stradale come grafo: Dijkstra, A* e raggiungibilità BFS/DFS disegnati a schermo.' }
];

export default {
  name: 'Landing',
  setup() {
    return { loggedIn: computed(() => isLoggedIn()), features: FEATURES };
  },
  template: `
    <div>
      <nav class="landing-nav sticky-top">
        <div class="container d-flex align-items-center justify-content-between py-2">
          <span class="d-flex align-items-center gap-2 fw-bold fs-5">
            <span class="brand-badge"><i class="bi bi-box-seam"></i></span> Logistics Engine
          </span>
          <div class="d-flex gap-2">
            <template v-if="loggedIn">
              <router-link to="/app" class="btn btn-brand btn-sm">Vai alla dashboard</router-link>
            </template>
            <template v-else>
              <router-link to="/login" class="btn btn-outline-secondary btn-sm">Accedi</router-link>
              <router-link to="/register" class="btn btn-brand btn-sm">Registrati</router-link>
            </template>
          </div>
        </div>
      </nav>

      <section class="hero">
        <div class="container py-5">
          <div class="row align-items-center g-5 py-4">
            <div class="col-lg-7">
              <span class="badge text-bg-light border mb-3">Software di gestione logistica</span>
              <h1 class="display-4 mb-3">Gestisci spedizioni, ordini e rotte.<span class="text-brand"> In un unico posto.</span></h1>
              <p class="fs-5 section-sub mb-4" style="max-width:36rem">
                Una piattaforma didattica costruita con DDD e Clean Architecture, che mostra dal vivo
                strutture dati e algoritmi: code, stack, alberi e grafi con percorsi minimi.
              </p>
              <div class="d-flex flex-wrap gap-2">
                <router-link to="/register" class="btn btn-brand btn-lg">Inizia ora</router-link>
                <router-link to="/login" class="btn btn-outline-secondary btn-lg">Ho già un account</router-link>
              </div>
            </div>
            <div class="col-lg-5">
              <div class="card p-3">
                <div class="card-body">
                  <div class="d-flex align-items-center gap-2 mb-3">
                    <span class="feature-icon"><i class="bi bi-share"></i></span>
                    <strong>Rete & percorsi minimi</strong>
                  </div>
                  <p class="section-sub small mb-3">Gli hub diventano nodi di un grafo, le strade archi pesati dal traffico. Dijkstra e A* trovano la rotta migliore, disegnata sulla mappa.</p>
                  <div class="d-flex gap-2 flex-wrap">
                    <span class="badge text-bg-primary">Dijkstra</span>
                    <span class="badge text-bg-primary">A*</span>
                    <span class="badge text-bg-secondary">BFS / DFS</span>
                    <span class="badge text-bg-secondary">Tree</span>
                    <span class="badge text-bg-secondary">Queue</span>
                    <span class="badge text-bg-secondary">Stack</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      <section class="container py-5">
        <div class="text-center mb-4">
          <h2 class="page-title h3">Tutto quello che serve</h2>
          <p class="section-sub">Ogni funzionalità è un algoritmo o una struttura dati mostrata dal vivo.</p>
        </div>
        <div class="row g-3">
          <div v-for="f in features" :key="f.title" class="col-md-6 col-lg-4">
            <div class="card h-100">
              <div class="card-body">
                <span class="feature-icon mb-3"><i class="bi" :class="f.icon"></i></span>
                <h3 class="h6 fw-bold">{{ f.title }}</h3>
                <p class="section-sub small mb-0">{{ f.text }}</p>
              </div>
            </div>
          </div>
        </div>
      </section>

      <footer class="border-top py-4">
        <div class="container d-flex flex-wrap justify-content-between align-items-center">
          <span class="section-sub small">Logistics Engine — progetto didattico DDD / Clean Architecture.</span>
          <router-link to="/register" class="btn btn-sm btn-brand">Crea un account</router-link>
        </div>
      </footer>
    </div>
  `
};
