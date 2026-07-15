import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { api, errorMessage } from '../api.js';
import { setAuth } from '../store.js';
import { toast } from '../toast.js';

const ROLES = ['Administrator', 'Manager', 'Operator', 'Viewer'];

export default {
  name: 'Register',
  setup() {
    const router = useRouter();
    const email = ref('');
    const password = ref('');
    const roleValue = ref('Manager');
    const busy = ref(false);

    async function submit() {
      busy.value = true;
      try {
        const auth = await api.post('/api/auth/register', {
          email: email.value,
          password: password.value,
          role: roleValue.value
        });
        setAuth(auth);
        toast(`Account creato: ${auth.email} (${auth.role})`);
        router.push('/app');
      } catch (error) {
        toast(errorMessage(error), 'danger');
      } finally {
        busy.value = false;
      }
    }

    return { email, password, roleValue, busy, roles: ROLES, submit };
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
              <h1 class="h4 fw-bold mb-1">Crea un account</h1>
              <p class="section-sub small mb-4">Scegli un ruolo per provare i diversi livelli di autorizzazione.</p>
              <form @submit.prevent="submit">
                <div class="mb-3">
                  <label class="form-label">Email</label>
                  <input class="form-control" type="email" v-model="email" required placeholder="admin@test.com" />
                </div>
                <div class="mb-3">
                  <label class="form-label">Password</label>
                  <input class="form-control" type="password" v-model="password" required minlength="8" placeholder="min. 8 caratteri" />
                </div>
                <div class="mb-3">
                  <label class="form-label">Ruolo</label>
                  <select class="form-select" v-model="roleValue">
                    <option v-for="r in roles" :key="r" :value="r">{{ r }}</option>
                  </select>
                </div>
                <button class="btn btn-brand w-100" :disabled="busy">
                  <span v-if="busy" class="spinner-border spinner-border-sm"></span>
                  <span v-else>Crea account</span>
                </button>
              </form>
              <p class="text-center section-sub small mt-3 mb-0">
                Hai già un account? <router-link to="/login">Accedi</router-link>
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  `
};
