import { ref, reactive, computed } from 'vue';
import { api, errorMessage } from '../api.js';
import { canManageData } from '../store.js';
import { toast } from '../toast.js';

export default {
  name: 'Network',
  setup() {
    const manage = computed(() => canManageData());

    const rawNodes = ref([]); // { hubId, hubCode, name, latitude, longitude }
    const rawEdges = ref([]); // { fromHubId, toHubId, distanceInKilometers, trafficFactor }
    const loaded = ref(false);

    const addForm = reactive({ fromHubId: '', toHubId: '', distance: 10, traffic: 1 });
    const routeForm = reactive({ origin: '', destination: '' });
    const routeResult = ref(null);
    const routeMode = ref('');

    const codeById = computed(() => {
      const map = {};
      rawNodes.value.forEach((n) => { map[n.hubId] = n.hubCode; });
      return map;
    });

    const highlightIds = computed(() => {
      const r = routeResult.value;
      if (!r) {
        return [];
      }
      const sequence = routeMode.value === 'reach' ? (r.path || []) : (r.nodes || []);
      return sequence.map((n) => n.hubId);
    });

    const highlightSet = computed(() => new Set(highlightIds.value));

    const pathPairs = computed(() => {
      const set = new Set();
      const ids = highlightIds.value;
      for (let i = 0; i < ids.length - 1; i++) {
        set.add(`${ids[i]}|${ids[i + 1]}`);
        set.add(`${ids[i + 1]}|${ids[i]}`);
      }
      return set;
    });

    const graphNodes = computed(() => rawNodes.value.map((n) => ({
      id: n.hubId,
      label: n.hubCode,
      title: `${n.hubCode} — ${n.name}`,
      color: highlightSet.value.has(n.hubId)
        ? { background: '#bbf7d0', border: '#16a34a' }
        : { background: '#c7d2fe', border: '#4f46e5' }
    })));

    const graphEdges = computed(() => rawEdges.value.map((e) => {
      const on = pathPairs.value.has(`${e.fromHubId}|${e.toHubId}`);
      return {
        from: e.fromHubId,
        to: e.toHubId,
        label: `${e.distanceInKilometers} km ×${e.trafficFactor}`,
        color: { color: on ? '#16a34a' : '#cbd5e1' },
        width: on ? 4 : 1
      };
    }));

    const pathCodes = computed(() => highlightIds.value.map((id) => codeById.value[id] || id.slice(0, 6)));
    const bfsCodes = computed(() => (routeResult.value?.breadthFirstOrder || []).map((id) => codeById.value[id] || id.slice(0, 6)));
    const dfsCodes = computed(() => (routeResult.value?.depthFirstOrder || []).map((id) => codeById.value[id] || id.slice(0, 6)));

    async function loadNetwork() {
      try {
        const data = await api.get('/api/roads/network');
        rawNodes.value = data.nodes;
        rawEdges.value = data.edges;
        loaded.value = true;
      } catch (error) {
        toast(errorMessage(error), 'danger');
      }
    }

    async function addRoad() {
      try {
        await api.post('/api/roads', {
          fromHubId: addForm.fromHubId,
          toHubId: addForm.toHubId,
          distanceInKilometers: parseFloat(addForm.distance),
          trafficFactor: parseFloat(addForm.traffic)
        });
        toast('Strada aggiunta');
        await loadNetwork();
      } catch (error) {
        toast(errorMessage(error), 'danger');
      }
    }

    async function runRoute(mode) {
      if (!routeForm.origin || !routeForm.destination) {
        toast('Seleziona origine e destinazione', 'danger');
        return;
      }
      const endpoint = mode === 'smart' ? '/api/routes/smart'
        : mode === 'reach' ? '/api/routes/reachability'
          : '/api/routes/shortest';
      try {
        const query = `?originHubId=${routeForm.origin}&destinationHubId=${routeForm.destination}`;
        const result = await api.get(`${endpoint}${query}`);
        routeMode.value = mode;
        routeResult.value = result;
        if (mode === 'reach' && !result.canReach) {
          toast('Destinazione non raggiungibile', 'info');
        }
      } catch (error) {
        routeResult.value = null;
        toast(errorMessage(error), 'danger');
      }
    }

    return {
      manage, rawNodes, loaded, addForm, routeForm, routeResult, routeMode,
      graphNodes, graphEdges, pathCodes, bfsCodes, dfsCodes,
      loadNetwork, addRoad, runRoute
    };
  },
  template: `
    <div>
      <h1 class="page-title h4 mb-1">Rete & Percorsi</h1>
      <p class="section-sub mb-4">Gli hub sono i nodi del grafo, le strade gli archi. Dijkstra, A* e BFS/DFS evidenziati sulla rete.</p>

      <div class="row g-3">
        <div class="col-lg-4">
          <div class="card mb-3"><div class="card-body">
            <h2 class="card-title mb-2">1 · Carica la rete</h2>
            <p class="section-sub small">Nodi (hub) e archi (strade) dal grafo.</p>
            <button class="btn btn-brand w-100" @click="loadNetwork"><i class="bi bi-arrow-repeat"></i> Carica / aggiorna</button>
          </div></div>

          <div class="card mb-3" :class="{ 'locked-card': !manage }"><div class="card-body">
            <h2 class="card-title mb-2">2 · Aggiungi strada</h2>
            <template v-if="manage">
              <form @submit.prevent="addRoad">
                <div class="mb-2">
                  <label class="form-label small">Hub di partenza</label>
                  <select class="form-select form-select-sm" v-model="addForm.fromHubId" required>
                    <option value="" disabled>Scegli…</option>
                    <option v-for="n in rawNodes" :key="n.hubId" :value="n.hubId">{{ n.hubCode }} — {{ n.name }}</option>
                  </select>
                </div>
                <div class="mb-2">
                  <label class="form-label small">Hub di arrivo</label>
                  <select class="form-select form-select-sm" v-model="addForm.toHubId" required>
                    <option value="" disabled>Scegli…</option>
                    <option v-for="n in rawNodes" :key="n.hubId" :value="n.hubId">{{ n.hubCode }} — {{ n.name }}</option>
                  </select>
                </div>
                <div class="row g-2">
                  <div class="col-6"><label class="form-label small">Distanza (km)</label><input class="form-control form-control-sm" type="number" step="0.1" min="0.1" v-model="addForm.distance" required /></div>
                  <div class="col-6"><label class="form-label small">Traffico (1–10)</label><input class="form-control form-control-sm" type="number" step="0.1" min="1" max="10" v-model="addForm.traffic" required /></div>
                </div>
                <button class="btn btn-brand btn-sm w-100 mt-3">Aggiungi</button>
              </form>
            </template>
            <div v-else class="alert alert-secondary small mb-0"><i class="bi bi-lock"></i> Riservato ad Administrator / Manager.</div>
          </div></div>

          <div class="card"><div class="card-body">
            <h2 class="card-title mb-2">3 · Calcola percorso</h2>
            <div class="mb-2">
              <label class="form-label small">Origine</label>
              <select class="form-select form-select-sm" v-model="routeForm.origin">
                <option value="" disabled>Scegli…</option>
                <option v-for="n in rawNodes" :key="n.hubId" :value="n.hubId">{{ n.hubCode }} — {{ n.name }}</option>
              </select>
            </div>
            <div class="mb-3">
              <label class="form-label small">Destinazione</label>
              <select class="form-select form-select-sm" v-model="routeForm.destination">
                <option value="" disabled>Scegli…</option>
                <option v-for="n in rawNodes" :key="n.hubId" :value="n.hubId">{{ n.hubCode }} — {{ n.name }}</option>
              </select>
            </div>
            <div class="d-grid gap-2">
              <button class="btn btn-outline-primary btn-sm" @click="runRoute('shortest')">Dijkstra (distanza)</button>
              <button class="btn btn-outline-primary btn-sm" @click="runRoute('smart')">A* (traffico)</button>
              <button class="btn btn-outline-primary btn-sm" @click="runRoute('reach')">Raggiungibilità (BFS/DFS)</button>
            </div>
          </div></div>
        </div>

        <div class="col-lg-8">
          <div class="card mb-3"><div class="card-body">
            <div class="d-flex justify-content-between align-items-center mb-2">
              <h2 class="card-title mb-0">Grafo della rete</h2>
              <span class="small section-sub">
                <span class="badge text-bg-primary">Hub</span>
                <span class="badge text-bg-success">Percorso</span>
              </span>
            </div>
            <graph-panel v-if="loaded" :nodes="graphNodes" :edges="graphEdges" />
            <p v-else class="section-sub small mb-0">Premi <em>Carica / aggiorna</em> per disegnare la rete.</p>
          </div></div>

          <div v-if="routeResult" class="card"><div class="card-body">
            <h2 class="card-title mb-2">Risultato</h2>
            <template v-if="routeMode !== 'reach'">
              <div class="d-flex flex-wrap gap-3 mb-2">
                <span class="badge text-bg-dark">{{ routeResult.strategy }}</span>
                <span>Costo totale: <strong>{{ routeResult.totalCost }}</strong></span>
                <span>Salti: <strong>{{ routeResult.hopCount }}</strong></span>
              </div>
              <div class="fw-semibold">{{ pathCodes.join('  →  ') }}</div>
            </template>
            <template v-else>
              <div class="mb-2">
                Raggiungibile:
                <span class="badge" :class="routeResult.canReach ? 'text-bg-success' : 'text-bg-danger'">{{ routeResult.canReach ? 'Sì' : 'No' }}</span>
              </div>
              <div v-if="routeResult.canReach" class="mb-2">Percorso (BFS): <strong>{{ pathCodes.join('  →  ') }}</strong></div>
              <div class="small"><span class="text-muted">Visita BFS:</span> {{ bfsCodes.join(' · ') }}</div>
              <div class="small"><span class="text-muted">Visita DFS:</span> {{ dfsCodes.join(' · ') }}</div>
            </template>
          </div></div>
        </div>
      </div>
    </div>
  `
};
