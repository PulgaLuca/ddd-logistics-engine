import { createRouter, createWebHashHistory } from 'vue-router';
import { isLoggedIn } from './store.js';

import Landing from './views/Landing.js';
import Login from './views/Login.js';
import Register from './views/Register.js';
import AppShell from './views/AppShell.js';
import Dashboard from './views/Dashboard.js';
import Shipments from './views/Shipments.js';
import Orders from './views/Orders.js';
import Dispatch from './views/Dispatch.js';
import Hubs from './views/Hubs.js';
import Zones from './views/Zones.js';
import Network from './views/Network.js';

const routes = [
  { path: '/', component: Landing, meta: { public: true } },
  { path: '/login', component: Login, meta: { public: true } },
  { path: '/register', component: Register, meta: { public: true } },
  {
    path: '/app',
    component: AppShell,
    children: [
      { path: '', component: Dashboard },
      { path: 'shipments', component: Shipments },
      { path: 'orders', component: Orders },
      { path: 'dispatch', component: Dispatch },
      { path: 'hubs', component: Hubs },
      { path: 'zones', component: Zones },
      { path: 'network', component: Network }
    ]
  },
  { path: '/:pathMatch(.*)*', redirect: '/' }
];

const router = createRouter({
  history: createWebHashHistory(),
  routes
});

router.beforeEach((to) => {
  if (!to.meta.public && !isLoggedIn()) {
    return { path: '/login' };
  }
  return true;
});

export default router;
