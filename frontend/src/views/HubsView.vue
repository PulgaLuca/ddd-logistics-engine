<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { hubsApi } from '../services/hubsApi'
import type { HubDto } from '../types/api'

const hubs = ref<HubDto[]>([])
const name = ref('')
const zone = ref('')
const error = ref('')
const loading = ref(false)
const saving = ref(false)

async function loadHubs() {
  loading.value = true
  error.value = ''
  try { hubs.value = await hubsApi.getAll() }
  catch (cause) { error.value = cause instanceof Error ? cause.message : 'Errore nel caricamento degli hub.' }
  finally { loading.value = false }
}

async function createHub() {
  saving.value = true
  error.value = ''
  try {
    await hubsApi.create({ name: name.value.trim(), zone: zone.value.trim() })
    name.value = ''
    zone.value = ''
    await loadHubs()
  } catch (cause) {
    error.value = cause instanceof Error ? cause.message : 'Errore nella creazione dell’hub.'
  } finally { saving.value = false }
}

onMounted(loadHubs)
</script>

<template>
  <section class="page-heading"><div><p class="eyebrow">Anagrafica</p><h1>Hub</h1><p>Elenco e creazione degli hub tramite API.</p></div><button class="button secondary" @click="loadHubs">Aggiorna</button></section>
  <p v-if="error" class="alert" role="alert">{{ error }}</p>
  <div class="grid">
    <section class="panel"><h2>Elenco hub</h2><p v-if="loading" class="muted">Caricamento…</p><p v-else-if="hubs.length === 0" class="muted">Nessun hub presente.</p>
      <ul v-else class="record-list"><li v-for="hub in hubs" :key="hub.id"><span><strong>{{ hub.name }}</strong><small>{{ hub.zone }}</small></span><code>{{ hub.id.slice(0, 8) }}</code></li></ul>
    </section>
    <section class="panel"><h2>Nuovo hub</h2><form class="form" @submit.prevent="createHub"><label>Nome<input v-model="name" required maxlength="100" placeholder="Milano Nord" /></label><label>Zona<input v-model="zone" required maxlength="100" placeholder="Lombardia" /></label><button class="button" :disabled="saving">{{ saving ? 'Salvataggio…' : 'Crea hub' }}</button></form></section>
  </div>
</template>
