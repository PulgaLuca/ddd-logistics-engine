import { ref, computed } from 'vue';
import { api, errorMessage } from '../api.js';
import { canOperate } from '../store.js';
import { toast } from '../toast.js';

export default {
  name: 'Dispatch',
  setup() {
    const operate = computed(() => canOperate());
    const history = ref(null);

    async function loadHistory() {
      try {
        history.value = await api.get('/api/dispatch/history');
      } catch (error) {
        toast(errorMessage(error), 'danger');
      }
    }
    async function undo() {
      try {
        history.value = await api.post('/api/dispatch/undo');
        toast('Undo eseguito');
      } catch (error) {
        toast(errorMessage(error), 'danger');
      }
    }
    async function redo() {
      try {
        history.value = await api.post('/api/dispatch/redo');
        toast('Redo eseguito');
      } catch (error) {
        toast(errorMessage(error), 'danger');
      }
    }

    return { operate, history, loadHistory, undo, redo };
  },
  template: `
    <div>
      <h1 class="page-title h4 mb-1">Dispatch — undo / redo</h1>
      <p class="section-sub mb-4">Cronologia delle assegnazioni ordine → hub, modellata come <strong>Stack</strong>.</p>

      <div class="d-flex gap-2 mb-3">
        <button class="btn btn-outline-primary" @click="loadHistory">Aggiorna stato</button>
        <button class="btn btn-brand" @click="undo" :disabled="!operate"><i class="bi bi-arrow-counterclockwise"></i> Undo</button>
        <button class="btn btn-brand" @click="redo" :disabled="!operate"><i class="bi bi-arrow-clockwise"></i> Redo</button>
      </div>
      <p v-if="!operate" class="section-sub small"><i class="bi bi-lock"></i> Undo/Redo riservati al ruolo Operator o superiore (puoi comunque vedere lo stato).</p>

      <div v-if="history" class="row g-3">
        <div class="col-md-6">
          <div class="card"><div class="card-body">
            <div class="d-flex justify-content-between align-items-center mb-2">
              <h2 class="card-title mb-0">Pila UNDO</h2>
              <span class="badge" :class="history.canUndo ? 'text-bg-success' : 'text-bg-secondary'">{{ history.canUndo ? 'disponibile' : 'vuota' }}</span>
            </div>
            <p class="section-sub small">La cima è l'ultima assegnazione applicata.</p>
            <div class="stack-col">
              <div v-for="(a, i) in history.undoStack" :key="'u'+i" class="stack-item">
                <div class="small"><strong>Ordine</strong> {{ a.orderId.slice(0, 8) }}…</div>
                <div class="small text-muted">Hub {{ a.hubId.slice(0, 8) }}…</div>
              </div>
            </div>
            <p v-if="!history.undoStack.length" class="section-sub small mb-0">Nessuna assegnazione.</p>
          </div></div>
        </div>
        <div class="col-md-6">
          <div class="card"><div class="card-body">
            <div class="d-flex justify-content-between align-items-center mb-2">
              <h2 class="card-title mb-0">Pila REDO</h2>
              <span class="badge" :class="history.canRedo ? 'text-bg-success' : 'text-bg-secondary'">{{ history.canRedo ? 'disponibile' : 'vuota' }}</span>
            </div>
            <p class="section-sub small">Assegnazioni annullate, pronte a essere riapplicate.</p>
            <div class="stack-col">
              <div v-for="(a, i) in history.redoStack" :key="'r'+i" class="stack-item">
                <div class="small"><strong>Ordine</strong> {{ a.orderId.slice(0, 8) }}…</div>
                <div class="small text-muted">Hub {{ a.hubId.slice(0, 8) }}…</div>
              </div>
            </div>
            <p v-if="!history.redoStack.length" class="section-sub small mb-0">Niente da ripetere.</p>
          </div></div>
        </div>
      </div>
      <p v-else class="section-sub">Premi <em>Aggiorna stato</em> per vedere le pile.</p>
    </div>
  `
};
