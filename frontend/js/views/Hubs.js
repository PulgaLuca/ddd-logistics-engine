import { ref, reactive, computed } from 'vue';
import { api, errorMessage } from '../api.js';
import { canManageData } from '../store.js';
import { toast } from '../toast.js';

export default {
  name: 'Hubs',
  setup() {
    const manage = computed(() => canManageData());
    const form = reactive({ code: '', name: '', latitude: 45.4642, longitude: 9.19 });
    const created = ref(null);
    const searchId = ref('');
    const found = ref(null);

    async function create() {
      try {
        const id = await api.post('/api/hubs', {
          code: form.code, name: form.name,
          latitude: parseFloat(form.latitude), longitude: parseFloat(form.longitude)
        });
        created.value = { id };
        toast('Hub registrato');
      } catch (error) {
        toast(errorMessage(error), 'danger');
      }
    }
    async function search() {
      try {
        found.value = await api.get(`/api/hubs/${searchId.value.trim()}`);
      } catch (error) {
        toast(errorMessage(error), 'danger');
      }
    }

    return { manage, form, created, searchId, found, create, search };
  },
  template: `
    <div>
      <h1 class="page-title h4 mb-1">Hub</h1>
      <p class="section-sub mb-4">I nodi della rete logistica. Copia l'ID per creare strade e calcolare percorsi.</p>
      <div class="row g-3">
        <div class="col-lg-6">
          <div class="card h-100" :class="{ 'locked-card': !manage }"><div class="card-body">
            <h2 class="card-title mb-3">Registra hub</h2>
            <template v-if="manage">
              <form @submit.prevent="create">
                <div class="row g-2">
                  <div class="col-md-4"><label class="form-label">Codice</label><input class="form-control" v-model="form.code" placeholder="HUB-0001" required /></div>
                  <div class="col-md-8"><label class="form-label">Nome</label><input class="form-control" v-model="form.name" placeholder="Milano Centrale" required /></div>
                  <div class="col-md-6"><label class="form-label">Latitudine</label><input class="form-control" type="number" step="0.0001" v-model="form.latitude" required /></div>
                  <div class="col-md-6"><label class="form-label">Longitudine</label><input class="form-control" type="number" step="0.0001" v-model="form.longitude" required /></div>
                </div>
                <button class="btn btn-brand mt-3">Registra</button>
              </form>
              <div v-if="created" class="mt-3"><data-view :data="created" /></div>
            </template>
            <div v-else class="alert alert-secondary small mb-0"><i class="bi bi-lock"></i> Riservato ad Administrator / Manager.</div>
          </div></div>
        </div>
        <div class="col-lg-6">
          <div class="card h-100"><div class="card-body">
            <h2 class="card-title mb-3">Cerca hub</h2>
            <form @submit.prevent="search" class="d-flex gap-2">
              <input class="form-control" v-model="searchId" placeholder="Hub id (guid)" required />
              <button class="btn btn-outline-primary">Cerca</button>
            </form>
            <div v-if="found" class="mt-3"><data-view :data="found" /></div>
          </div></div>
        </div>
      </div>
    </div>
  `
};
