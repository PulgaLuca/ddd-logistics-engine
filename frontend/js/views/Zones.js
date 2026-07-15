import { ref, reactive, computed } from 'vue';
import { api, errorMessage } from '../api.js';
import { canManageData, canOperate } from '../store.js';
import { toast } from '../toast.js';

// Trasforma l'albero delle zone in nodi/archi per la visualizzazione gerarchica.
function buildTreeGraph(rootNode) {
  const nodes = [];
  const edges = [];
  const walk = (node) => {
    nodes.push({ id: node.zoneId, label: `${node.code}\n${node.name}`, color: { background: '#c7d2fe', border: '#4f46e5' } });
    (node.streets || []).forEach((street, i) => {
      const streetId = `${node.zoneId}-st-${i}`;
      nodes.push({ id: streetId, label: street.name, shape: 'box', color: { background: '#dcfce7', border: '#16a34a' }, font: { size: 12 } });
      edges.push({ from: node.zoneId, to: streetId, dashes: true });
    });
    (node.subzones || []).forEach((child) => {
      edges.push({ from: node.zoneId, to: child.zoneId });
      walk(child);
    });
  };
  walk(rootNode);
  return { nodes, edges };
}

export default {
  name: 'Zones',
  setup() {
    const manage = computed(() => canManageData());
    const operate = computed(() => canOperate());

    const form = reactive({ code: '', name: '', parentZoneId: '' });
    const created = ref(null);
    const street = reactive({ zoneId: '', streetName: '', postalCode: '' });
    const searchId = ref('');
    const found = ref(null);
    const treeId = ref('');
    const treeNodes = ref([]);
    const treeEdges = ref([]);
    const treeLoaded = ref(false);

    async function create() {
      try {
        const id = await api.post('/api/zones', {
          code: form.code, name: form.name,
          parentZoneId: form.parentZoneId.trim() === '' ? null : form.parentZoneId.trim()
        });
        created.value = { id };
        toast('Zona creata');
      } catch (error) {
        toast(errorMessage(error), 'danger');
      }
    }
    async function addStreet() {
      try {
        await api.post(`/api/zones/${street.zoneId.trim()}/streets`, { streetName: street.streetName, postalCode: street.postalCode });
        toast('Via aggiunta');
      } catch (error) {
        toast(errorMessage(error), 'danger');
      }
    }
    async function search() {
      try {
        found.value = await api.get(`/api/zones/${searchId.value.trim()}`);
      } catch (error) {
        toast(errorMessage(error), 'danger');
      }
    }
    async function loadTree() {
      try {
        const tree = await api.get(`/api/zones/${treeId.value.trim()}/tree`);
        const graph = buildTreeGraph(tree);
        treeNodes.value = graph.nodes;
        treeEdges.value = graph.edges;
        treeLoaded.value = true;
      } catch (error) {
        toast(errorMessage(error), 'danger');
      }
    }

    return { manage, operate, form, created, street, searchId, found, treeId, treeNodes, treeEdges, treeLoaded, create, addStreet, search, loadTree };
  },
  template: `
    <div>
      <h1 class="page-title h4 mb-1">Zone</h1>
      <p class="section-sub mb-4">Albero delle zone di consegna: Zona → Sottozona → Vie.</p>
      <div class="row g-3">

        <div class="col-lg-6">
          <div class="card h-100" :class="{ 'locked-card': !manage }"><div class="card-body">
            <h2 class="card-title mb-3">Crea zona</h2>
            <template v-if="manage">
              <form @submit.prevent="create">
                <div class="row g-2">
                  <div class="col-md-4"><label class="form-label">Codice</label><input class="form-control" v-model="form.code" placeholder="ZON-0001" required /></div>
                  <div class="col-md-4"><label class="form-label">Nome</label><input class="form-control" v-model="form.name" placeholder="Lombardia" required /></div>
                  <div class="col-md-4"><label class="form-label">Parent (opz.)</label><input class="form-control" v-model="form.parentZoneId" placeholder="zone id" /></div>
                </div>
                <button class="btn btn-brand mt-3">Crea</button>
              </form>
              <div v-if="created" class="mt-3"><data-view :data="created" /></div>
            </template>
            <div v-else class="alert alert-secondary small mb-0"><i class="bi bi-lock"></i> Riservato ad Administrator / Manager.</div>
          </div></div>
        </div>

        <div class="col-lg-6">
          <div class="card mb-3" :class="{ 'locked-card': !operate }"><div class="card-body">
            <h2 class="card-title mb-3">Aggiungi via</h2>
            <template v-if="operate">
              <form @submit.prevent="addStreet">
                <div class="row g-2">
                  <div class="col-md-4"><input class="form-control" v-model="street.zoneId" placeholder="Zone id" required /></div>
                  <div class="col-md-4"><input class="form-control" v-model="street.streetName" placeholder="Via Roma" required /></div>
                  <div class="col-md-4"><input class="form-control" v-model="street.postalCode" placeholder="20100" required /></div>
                </div>
                <button class="btn btn-brand mt-3">Aggiungi</button>
              </form>
            </template>
            <div v-else class="alert alert-secondary small mb-0"><i class="bi bi-lock"></i> Riservato al ruolo Operator o superiore.</div>
          </div></div>

          <div class="card"><div class="card-body">
            <h2 class="card-title mb-3">Cerca zona</h2>
            <form @submit.prevent="search" class="d-flex gap-2">
              <input class="form-control" v-model="searchId" placeholder="Zone id (guid)" required />
              <button class="btn btn-outline-primary">Cerca</button>
            </form>
            <div v-if="found" class="mt-3"><data-view :data="found" /></div>
          </div></div>
        </div>

        <div class="col-12">
          <div class="card"><div class="card-body">
            <div class="d-flex flex-wrap justify-content-between align-items-center mb-3 gap-2">
              <h2 class="card-title mb-0">Albero della zona</h2>
              <form @submit.prevent="loadTree" class="d-flex gap-2">
                <input class="form-control form-control-sm" v-model="treeId" placeholder="Zone id radice" required />
                <button class="btn btn-sm btn-brand">Visualizza albero</button>
              </form>
            </div>
            <div v-if="treeLoaded">
              <div class="small section-sub mb-2">
                <span class="badge text-bg-primary">Zone</span>
                <span class="badge text-bg-success">Vie</span>
              </div>
              <graph-panel :nodes="treeNodes" :edges="treeEdges" :hierarchical="true" :directed="true" />
            </div>
            <p v-else class="section-sub small mb-0">Inserisci l'id di una zona radice per disegnarne l'albero.</p>
          </div></div>
        </div>

      </div>
    </div>
  `
};
