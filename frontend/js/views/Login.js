import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { api, errorMessage } from '../api.js';
import { setAuth } from '../store.js';
import { toast } from '../toast.js';
import { getApiBase, setApiBase } from '../config.js';

export default {
  name: 'Login',
  setup() {
    const router = useRouter();
    const email = ref('');
    const password = ref('');
    const busy = ref(false);
    const apiBase = ref(getApiBase());

    async function submit() {
      busy.value = true;
      try {
        const auth = await api.post('/api/auth/login', { email: email.value, password: password.value });
        setAuth(auth);
        toast(`Bentornato, ${auth.email}`);
        router.push('/app');
      } catch (error) {
        toast(errorMessage(error), 'danger');
      } finally {
        busy.value = false;
      }
    }

    function saveApi() {
      setApiBase(apiBase.value);
      apiBase.value = getApiBase();
      toast('URL API salvato');
    }

    return { email, password, busy, apiBase, submit, saveApi };
  },
  template: `
    <div class="container">
      <div class="row justify-content-center py-5">
        <div class="col-sm-9 col-md-6 col-lg-5">
          <router-link to="/" class="d-inline-flex align-items-center gap-2 fw-bold text-decoration-none mb-4">
            <span class="brand-badge"><i class="bi bi-box-seam"></i></span> Logistics Engine
          </router-link>
          <div class="card">
            <div class="card-body p-4">
              <h1 class="h4 fw-bold mb-1">Accedi</h1>
              <p class="section-sub small mb-4">Inserisci le tue credenziali per continuare.</p>
              <form @submit.prevent="submit">
                <div class="mb-3">
                  <label class="form-label">Email</label>
                  <input class="form-control" type="email" v-model="email" required placeholder="admin@test.com" />
                </div>
                <div class="mb-3">
                  <label class="form-label">Password</label>
                  <input class="form-control" type="password" v-model="password" required placeholder="password123" />
                </div>
                <button class="btn btn-brand w-100" :disabled="busy">
                  <span v-if="busy" class="spinner-border spinner-border-sm"></span>
                  <span v-else>Accedi</span>
                </button>
              </form>
              <p class="text-center section-sub small mt-3 mb-0">
                Non hai un account? <router-link to="/register">Registrati</router-link>
              </p>
            </div>
          </div>
          <details class="mt-3">
            <summary class="section-sub small">Impostazioni avanzate (URL API)</summary>
            <div class="input-group input-group-sm mt-2">
              <input class="form-control" v-model="apiBase" placeholder="https://localhost:7086" />
              <button class="btn btn-outline-secondary" @click="saveApi">Salva</button>
            </div>
          </details>
        </div>
      </div>
    </div>
  `
};
