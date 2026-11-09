import { createRouter, createWebHistory } from 'vue-router'

// Il router mostra viste diverse senza ricaricare la pagina e carica ogni sezione solo quando serve.
export const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', redirect: '/overview' },
    { path: '/overview', component: () => import('../views/OverviewView.vue') },
    { path: '/hubs', component: () => import('../views/HubsView.vue') },
    { path: '/shipments', component: () => import('../views/ShipmentsView.vue') },
    { path: '/network', component: () => import('../views/NetworkView.vue') },
    { path: '/profile', component: () => import('../views/ProfileView.vue') },
  ],
})
