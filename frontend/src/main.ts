import { createApp } from 'vue'
import App from './App.vue'
import { router } from './router'
import './style.css'

// Crea l'applicazione Vue e collega il router prima di renderizzare la pagina.
createApp(App).use(router).mount('#app')
