import { ref, reactive, computed } from 'vue';
import { api, errorMessage } from '../api.js';
import { canManageData, canOperate } from '../store.js';
import { toast } from '../toast.js';

export default {
  name: 'Orders',
  setup() {
    const manage = computed(() => canManageData());
    const operate = computed(() => canOperate());

    const form = reactive({ orderNumber: '', customerName: '', priority: 'Standard' });
    const created = ref(null);
    const searchId = ref('');
    const found = ref(null);
    const assign = reactive({ orderId: '', hubId: '' });
    const fifo = ref(null);
    const priority = ref(null);

    async function create() {
      try {
        const id = await api.post('/api/orders', { orderNumber: form.orderNumber, customerName: form.customerName, priority: form.priority });
        created.value = { id };
        toast('Ordine registrato');
      } catch (error) {
        toast(errorMessage(error), 'danger');
      }
    }
    async function search() {
      try {
        found.value = await api.get(`/api/orders/${searchId.value.trim()}`);
      } catch (error) {
        toast(errorMessage(error), 'danger');
      }
    }
    async function doAssign() {
      try {
        await api.post(`/api/orders/${assign.orderId.trim()}/assign`, { hubId: assign.hubId.trim() });
        toast('Ordine assegnato');
      } catch (error) {
        toast(errorMessage(error), 'danger');
      }
    }
    async function loadFifo() {
      try {
        fifo.value = await api.get('/api/orders/processing-queue');
      } catch (error) {
        toast(errorMessage(error), 'danger');
      }
    }
    async function loadPriority() {
      try {
        priority.value = await api.get('/api/orders/express-queue');
      } catch (error) {
        toast(errorMessage(error), 'danger');
      }
    }

    return { manage, operate, form, created, searchId, found, assign, fifo, priority, create, search, doAssign, loadFifo, loadPriority };
  },
  template: `
    <div>
      <h1 class="page-title h4 mb-1">Ordini</h1>
      <p class="section-sub mb-4">Registra ordini, assegnali agli hub e osserva le code (FIFO e a priorità).</p>
      <div class="row g-3">

        <div class="col-lg-6">
          <div class="card h-100" :class="{ 'locked-card': !manage }"><div class="card-body">
            <h2 class="card-title mb-3">Registra ordine</h2>
            <template v-if="manage">
              <form @submit.prevent="create">
                <div class="row g-2">
                  <div class="col-md-6"><label class="form-label">Order number</label><input class="form-control" v-model="form.orderNumber" placeholder="ORD-ABC12345" required /></div>
                  <div class="col-md-6"><label class="form-label">Cliente</label><input class="form-control" v-model="form.customerName" placeholder="Mario Rossi" required /></div>
                  <div class="col-md-6"><label class="form-label">Priorità</label>
                    <select class="form-select" v-model="form.priority">
                      <option>Standard</option><option>Express</option><option>Priority</option>
                    </select>
                  </div>
                </div>
                <button class="btn btn-brand mt-3">Registra</button>
              </form>
              <div v-if="created" class="mt-3"><data-view :data="created" /></div>
            </template>
            <div v-else class="alert alert-secondary small mb-0"><i class="bi bi-lock"></i> Riservato ad Administrator / Manager.</div>
          </div></div>
        </div>

        <div class="col-lg-6">
          <div class="card mb-3"><div class="card-body">
            <h2 class="card-title mb-3">Cerca ordine</h2>
            <form @submit.prevent="search" class="d-flex gap-2">
              <input class="form-control" v-model="searchId" placeholder="Order id (guid)" required />
              <button class="btn btn-outline-primary">Cerca</button>
            </form>
            <div v-if="found" class="mt-3"><data-view :data="found" /></div>
          </div></div>

          <div class="card" :class="{ 'locked-card': !operate }"><div class="card-body">
            <h2 class="card-title mb-3">Assegna a un hub</h2>
            <template v-if="operate">
              <form @submit.prevent="doAssign">
                <div class="row g-2">
                  <div class="col-md-6"><input class="form-control" v-model="assign.orderId" placeholder="Order id (guid)" required /></div>
                  <div class="col-md-6"><input class="form-control" v-model="assign.hubId" placeholder="Hub id (guid)" required /></div>
                </div>
                <button class="btn btn-brand mt-3">Assegna</button>
              </form>
            </template>
            <div v-else class="alert alert-secondary small mb-0"><i class="bi bi-lock"></i> Riservato al ruolo Operator o superiore.</div>
          </div></div>
        </div>

        <div class="col-12">
          <div class="card"><div class="card-body">
            <div class="d-flex justify-content-between align-items-center mb-3">
              <h2 class="card-title mb-0">Code degli ordini in attesa</h2>
              <div class="d-flex gap-2">
                <button class="btn btn-sm btn-outline-primary" @click="loadFifo">Coda FIFO</button>
                <button class="btn btn-sm btn-outline-primary" @click="loadPriority">Coda a priorità</button>
              </div>
            </div>
            <div class="row g-3">
              <div class="col-lg-6" v-if="fifo"><queue-viz title="FIFO — ordine di arrivo (Queue)" :items="fifo.items" /></div>
              <div class="col-lg-6" v-if="priority"><queue-viz title="Priorità — Express delivery (Priority Queue)" :items="priority.items" /></div>
            </div>
            <p v-if="!fifo && !priority" class="section-sub small mb-0">Premi un pulsante per caricare la coda.</p>
          </div></div>
        </div>

      </div>
    </div>
  `
};
