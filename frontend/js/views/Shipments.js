import { ref, reactive, computed } from 'vue';
import { api, errorMessage } from '../api.js';
import { canManageData, canOperate } from '../store.js';
import { toast } from '../toast.js';

export default {
  name: 'Shipments',
  setup() {
    const manage = computed(() => canManageData());
    const operate = computed(() => canOperate());

    const form = reactive({
      trackingNumber: '', weight: 10, isExpress: false,
      oStreet: '', oCity: '', oPostal: '', oCountry: 'IT',
      dStreet: '', dCity: '', dPostal: '', dCountry: 'IT'
    });
    const created = ref(null);
    const searchId = ref('');
    const found = ref(null);
    const dispatchId = ref('');

    async function create() {
      try {
        const id = await api.post('/api/shipments', {
          trackingNumber: form.trackingNumber,
          weightInKilograms: parseFloat(form.weight),
          isExpress: form.isExpress,
          origin: { street: form.oStreet, city: form.oCity, postalCode: form.oPostal, country: form.oCountry },
          destination: { street: form.dStreet, city: form.dCity, postalCode: form.dPostal, country: form.dCountry }
        });
        created.value = { id };
        toast('Spedizione creata');
      } catch (error) {
        toast(errorMessage(error), 'danger');
      }
    }

    async function search() {
      try {
        found.value = await api.get(`/api/shipments/${searchId.value.trim()}`);
      } catch (error) {
        toast(errorMessage(error), 'danger');
      }
    }

    async function doDispatch() {
      try {
        await api.post(`/api/shipments/${dispatchId.value.trim()}/dispatch`);
        toast('Spedizione messa in transito');
      } catch (error) {
        toast(errorMessage(error), 'danger');
      }
    }

    return { manage, operate, form, created, searchId, found, dispatchId, create, search, doDispatch };
  },
  template: `
    <div>
      <h1 class="page-title h4 mb-1">Spedizioni</h1>
      <p class="section-sub mb-4">Crea, consulta e spedisci le spedizioni.</p>
      <div class="row g-3">

        <div class="col-lg-6">
          <div class="card h-100" :class="{ 'locked-card': !manage }"><div class="card-body">
            <h2 class="card-title mb-3">Crea spedizione</h2>
            <template v-if="manage">
              <form @submit.prevent="create">
                <div class="row g-2">
                  <div class="col-md-6"><label class="form-label">Tracking number</label><input class="form-control" v-model="form.trackingNumber" placeholder="TRK-ABC12345" required /></div>
                  <div class="col-md-3"><label class="form-label">Peso (kg)</label><input class="form-control" type="number" step="0.1" min="0.1" v-model="form.weight" required /></div>
                  <div class="col-md-3 d-flex align-items-end pb-2"><div class="form-check"><input class="form-check-input" type="checkbox" v-model="form.isExpress" id="exp" /><label class="form-check-label" for="exp">Express</label></div></div>
                </div>
                <div class="row g-2 mt-1">
                  <div class="col-12"><span class="small text-muted fw-semibold">Origine</span></div>
                  <div class="col-md-6"><input class="form-control" v-model="form.oStreet" placeholder="Via" required /></div>
                  <div class="col-md-6"><input class="form-control" v-model="form.oCity" placeholder="Città" required /></div>
                  <div class="col-md-6"><input class="form-control" v-model="form.oPostal" placeholder="CAP" required /></div>
                  <div class="col-md-6"><input class="form-control" v-model="form.oCountry" placeholder="Paese" required /></div>
                </div>
                <div class="row g-2 mt-1">
                  <div class="col-12"><span class="small text-muted fw-semibold">Destinazione</span></div>
                  <div class="col-md-6"><input class="form-control" v-model="form.dStreet" placeholder="Via" required /></div>
                  <div class="col-md-6"><input class="form-control" v-model="form.dCity" placeholder="Città" required /></div>
                  <div class="col-md-6"><input class="form-control" v-model="form.dPostal" placeholder="CAP" required /></div>
                  <div class="col-md-6"><input class="form-control" v-model="form.dCountry" placeholder="Paese" required /></div>
                </div>
                <button class="btn btn-brand mt-3">Crea</button>
              </form>
              <div v-if="created" class="mt-3"><data-view :data="created" /></div>
            </template>
            <div v-else class="alert alert-secondary small mb-0"><i class="bi bi-lock"></i> Riservato ad Administrator / Manager.</div>
          </div></div>
        </div>

        <div class="col-lg-6">
          <div class="card mb-3"><div class="card-body">
            <h2 class="card-title mb-3">Cerca spedizione</h2>
            <form @submit.prevent="search" class="d-flex gap-2">
              <input class="form-control" v-model="searchId" placeholder="Shipment id (guid)" required />
              <button class="btn btn-outline-primary">Cerca</button>
            </form>
            <div v-if="found" class="mt-3"><data-view :data="found" /></div>
          </div></div>

          <div class="card" :class="{ 'locked-card': !operate }"><div class="card-body">
            <h2 class="card-title mb-3">Spedisci (dispatch)</h2>
            <template v-if="operate">
              <form @submit.prevent="doDispatch" class="d-flex gap-2">
                <input class="form-control" v-model="dispatchId" placeholder="Shipment id (guid)" required />
                <button class="btn btn-brand">Spedisci</button>
              </form>
            </template>
            <div v-else class="alert alert-secondary small mb-0"><i class="bi bi-lock"></i> Riservato al ruolo Operator o superiore.</div>
          </div></div>
        </div>

      </div>
    </div>
  `
};
