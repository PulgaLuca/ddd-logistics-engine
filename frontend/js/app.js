import { createApp } from 'vue';
import router from './router.js';
import App from './root.js';
import DataView from './components/DataView.js';
import GraphPanel from './components/GraphPanel.js';
import QueueViz from './components/QueueViz.js';

const app = createApp(App);

// Componenti riutilizzabili registrati globalmente (DataView è ricorsivo).
app.component('DataView', DataView);
app.component('GraphPanel', GraphPanel);
app.component('QueueViz', QueueViz);

app.use(router);
app.mount('#app');
