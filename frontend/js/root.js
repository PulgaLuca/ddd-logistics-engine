import { toasts } from './toast.js';

export default {
  name: 'RootApp',
  setup() {
    return { toasts };
  },
  template: `
    <router-view />
    <div class="toast-stack">
      <div v-for="t in toasts" :key="t.id" class="app-toast" :class="t.type">{{ t.message }}</div>
    </div>
  `
};
