// Rappresentazione visiva di una coda (FIFO o a priorità): carte in sequenza.
export default {
  name: 'QueueViz',
  props: {
    title: { default: '' },
    items: { default: () => [] }
  },
  template: `
    <div>
      <div class="d-flex justify-content-between align-items-center mb-2">
        <strong class="small text-muted">{{ title }}</strong>
        <span class="badge text-bg-light">{{ items.length }} in coda</span>
      </div>
      <div v-if="items.length" class="queue-track">
        <div v-for="item in items" :key="item.orderId" class="queue-item">
          <div class="pos">#{{ item.position }} · {{ item.priority }}</div>
          <div class="fw-semibold">{{ item.orderNumber }}</div>
          <div class="small text-muted">{{ item.customerName }}</div>
        </div>
      </div>
      <div v-else class="text-muted small">Nessun ordine in attesa.</div>
    </div>
  `
};
