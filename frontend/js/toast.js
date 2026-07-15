// Servizio di notifiche (toast) reattivo, condiviso da tutta l'app.
import { reactive } from 'vue';

export const toasts = reactive([]);
let counter = 0;

export function toast(message, type = 'success') {
  const item = { id: ++counter, message, type };
  toasts.push(item);
  setTimeout(() => {
    const index = toasts.findIndex((t) => t.id === item.id);
    if (index >= 0) {
      toasts.splice(index, 1);
    }
  }, 4200);
}
