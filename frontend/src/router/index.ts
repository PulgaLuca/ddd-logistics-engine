import { createRouter, createWebHistory } from 'vue-router'

// La navigazione cambia vista nel browser senza ricaricare l'intera applicazione.
export const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', redirect: '/hubs' },
    { path: '/hubs', component: () => import('../views/HubsView.vue') },
    { path: '/shipments', component: () => import('../views/ShipmentsView.vue') },
  ],
})
